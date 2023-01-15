using App.Reversi.Shared;
using Grpc.Core;
using MagicOnion.Client;
using UnityEngine;

namespace App.Reversi
{
    public class SampleAppCall : MonoBehaviour
    {
        void Start()
        {
            CallTest();
        }

        private async void CallTest()
        {
            var channel = new Channel("localhost", 5000, ChannelCredentials.Insecure);
            var client = MagicOnionClient.Create<IMyFirstService>(channel);
            var result = await client.SumAsync(1, 2);
            Debug.Log($"Result: {result}");
            await channel.ShutdownAsync();
        }
    }
}