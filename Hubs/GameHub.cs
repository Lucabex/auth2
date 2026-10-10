using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace auth2.Hubs;

public class GameHub : Hub
{
    private static readonly ConcurrentQueue<string> _awaitingPlayer = new ConcurrentQueue<string>();
    private static readonly ConcurrentDictionary<string,string> _connectionToRoom = new ConcurrentDictionary<string,string>();

    public async Task SearchForMatch (string userName)
    {
        if(_awaitingPlayer.TryDequeue(out string opponentConnectionId))
        {
            string roomId = Guid.NewGuid().ToString();
            _connectionToRoom[Context.ConnectionId]= roomId;
            _connectionToRoom[opponentConnectionId]= roomId;

            await Groups.AddToGroupAsync(Context.ConnectionId,roomId);
            await Groups.AddToGroupAsync(opponentConnectionId,roomId);
            //notify opponent that match is found

            await Clients.Client(opponentConnectionId).SendAsync("MatchFound",roomId,userName);

            await Clients.Caller.SendAsync("MatchFound",roomId,"opponent");

        }
        else
        {
            _awaitingPlayer.Enqueue(Context.ConnectionId);
            await Clients.Caller.SendAsync("WaitingForMatch");
        }

    }
    //make a  move and send it to opponent FE
    public async Task MakeOnlineMove(string roomId,object moveData)
    {
        await Clients.OthersInGroup(roomId).SendAsync("OpponentMove",moveData);
    } 

    public async Task CancelSearch()
    {
        _connectionToRoom.TryRemove(Context.ConnectionId,out _);
    }




}