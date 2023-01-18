using App.Reversi.Shared;
using MagicOnion.Server.Hubs;
using UnityEngine;

namespace UnityReversi.Server.Services
{
    public class GamingHub : StreamingHubBase<IGamingHub, IGamingHubReceiver>, IGamingHub
    {
        private IGroup room;
        private Player self;
        private IInMemoryStorage<Player> storage;

        public async ValueTask<Player[]> JoinAsync(string roomName, string userName, Vector3 position, Quaternion rotation)
        {
            self = new Player() { Name = userName, Position = position, Rotation = rotation };

            (room, storage) = await Group.AddAsync(roomName, self);
            
            Broadcast(room).OnJoin(self);

            return storage.AllValues.ToArray();
        }

        public async ValueTask LeaveAsync()
        {
            await room.RemoveAsync(Context);
            Broadcast(room).OnLeave(self);
        }

        public ValueTask MoveAsync(Vector3 position, Quaternion rotation)
        {
            self.Position = position;
            self.Rotation = rotation;
            Broadcast(room).OnMove(self);
            return ValueTask.CompletedTask;
        }

        protected override ValueTask OnDisconnected()
        {
            return ValueTask.CompletedTask;
        }
    }
}