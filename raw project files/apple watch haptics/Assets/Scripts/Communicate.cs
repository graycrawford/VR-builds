using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Collections.Concurrent;

public class ThreadedCommunicator
{
    private readonly Thread _pushWorker;
    private readonly Thread _pullWorker;

    private bool _pushCancelled;
    private bool _pullCancelled;

    public delegate void MessageDelegate(byte[] message);

    private readonly MessageDelegate _messageSent;
    //private readonly MessageDelegate _messageRecv;

    public readonly ConcurrentQueue<(byte[], System.Action)> sendMessage = new ConcurrentQueue<(byte[], System.Action)>();

    public readonly ConcurrentQueue<byte[]> recvMessage = new ConcurrentQueue<byte[]>();

    private readonly ConcurrentQueue<System.Action> callbacks = new ConcurrentQueue<System.Action>();

    private string _pushAddr;
    private string _pullAddr;

    private System.TimeSpan timeout;

    public ThreadedCommunicator(MessageDelegate messageDelegate,
        string pushAddr, string pullAddr, System.TimeSpan timeout, int recvHWM = 2) {
        AsyncIO.ForceDotNet.Force();

        _pushAddr = pushAddr;
        _pullAddr = pullAddr;
        this.timeout = timeout;

        _messageSent = messageDelegate;
        _pushWorker = new Thread(DoPush);
        _pullWorker = new Thread(DoPull);

    }

    // runs in another thread
    private void DoPush() {
        var msg = new NetMQ.Msg();

        using (var subSocket = new NetMQ.Sockets.PushSocket()) {
            subSocket.Options.SendHighWatermark = 1;
            subSocket.Connect(_pushAddr);
            while (!_pushCancelled) {
                if (sendMessage.TryDequeue(out (byte[], System.Action) msgData)) {
                    msg.InitEmpty();
                    try {
                        var (to_send, cb) = msgData;
                        var dummyMsg = new NetMQ.Msg();

                        dummyMsg.InitGC(to_send, to_send.Length);
                        if (subSocket.TrySend(ref dummyMsg, timeout, false)) {
                            callbacks.Enqueue(cb);
                        }
                    } catch (NetMQ.FiniteStateMachineException exn) {
                        callbacks.Enqueue(() => Debug.LogError(
                            string.Format("FSM EXN {0}", exn.ToString()
                        )));
                    }
                }
                else {
                    Thread.Sleep(1);
                }
            }
            subSocket.Close();
        }
    }

    private void DoPull() {

        using (var subSocket = new NetMQ.Sockets.PullSocket()) {
            subSocket.Connect(_pullAddr);
            while (!_pullCancelled) {
                var recvMsg = new NetMQ.Msg();
                recvMsg.InitEmpty();
                if(subSocket.TryReceive(ref recvMsg, timeout)) {
                    recvMessage.Enqueue(recvMsg.Data);
                } else {
                    Thread.Sleep(1);
                }
            }
            subSocket.Close();
        }
    }

    public void Update() {
        while (!callbacks.IsEmpty) {
            if (callbacks.TryDequeue(out System.Action callback)) {
                callback();
            }
            else {
                break;
            }
        }
    }

    public void Start() {
        _pushCancelled = false;
        _pullCancelled = false;
        _pushWorker.Start();
        _pullWorker.Start();
    }

    public void Stop() {
        _pushCancelled = true;
        _pullCancelled = true;
        _pushWorker.Join();
        _pullWorker.Join();
        NetMQ.NetMQConfig.Cleanup();
    }
}


public class Communicate : MonoBehaviour
{

    public string push_to = "tcp://teenie.cfa.cmu.edu:8686";
    public string pull_from = "tcp://teenie.cfa.cmu.edu:8687";
    public int size = 256;
    [Range(0.02f, 1.0f)]
    public float truncation = 0.4f;
    public float[] categories = new float[1000];
    public float[] zVector = new float[128];
    public Texture2D output_texture;
    Color32[] output_colors;

    private ThreadedCommunicator comm;
    // Start is called before the first frame update
    void Start() {
        AsyncIO.ForceDotNet.Force();
        output_colors = new Color32[size * size];
        Debug.Log(output_colors.Length * 4);
        output_texture = new Texture2D(size, size, TextureFormat.RGBA32, false);

        comm = new ThreadedCommunicator(CopyReplyToTexture, push_to, pull_from, System.TimeSpan.FromSeconds(1));
        comm.Start();
    }

    void Update() {
        comm.Update();
        while(!comm.recvMessage.IsEmpty) {
            if(comm.recvMessage.TryDequeue(out byte[] result)) {
                CopyReplyToTexture(result);
            }
        }
    }

    public bool Send(string msg, System.Action cb, bool queue = false) {
        if (!queue && !comm.sendMessage.IsEmpty) return false;

        byte[] to_send = new byte[msg.Length];
        for (int i = 0; i < msg.Length; i++) to_send[i] = (byte)msg[i];
        comm.sendMessage.Enqueue((to_send, cb));
        return true;
    }

    void CopyReplyToTexture(byte[] reply_bytes) {
        float t2 = Time.realtimeSinceStartup;
        if (reply_bytes.Length != output_colors.Length * 4) {
            Debug.LogError("expected " + (output_colors.Length * 4).ToString()
                            + "bytes but got " + reply_bytes.Length.ToString());
        }

        unsafe {
            fixed (Color32* colors_ptr = output_colors)
            fixed (byte* bytes_ptr = reply_bytes) {
                long* color_as_int = (long*)colors_ptr;
                long* bytes_as_int = (long*)bytes_ptr;

                int len = output_colors.Length / (sizeof(long) / sizeof(Color32));
                for (int i = 0; i < len; i++) {
                    color_as_int[i] = bytes_as_int[i];
                }
            }
        }

        output_texture.SetPixels32(output_colors);
        output_texture.Apply();

        Debug.Log(string.Format("copy time {0}", Time.realtimeSinceStartup - t2));

    }

    private void OnDestroy() {
        comm.Stop();
    }

}

