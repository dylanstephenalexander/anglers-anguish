using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

//To Do: Implement AuthenticationService

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;
    private void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    ISession activeSession;
    ISession ActiveSession {
        get => activeSession;
        set{
            activeSession = value;
            Debug.Log($"Active session: {activeSession}");
        }
    }

    async public void Start(){
        try{
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services initialized, ready to create or join sessions");
        }
        catch(Exception e){
            Debug.LogException(e);
        }
    }

    async public void StartSessionAsHost(){
        var options = new SessionOptions {
            MaxPlayers = 6, 
            IsLocked = false,
            IsPrivate = false
        }.WithRelayNetwork();
        ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
        Debug.Log($"Session {ActiveSession.Id} created! Join code: {ActiveSession.Code}");
        UnityEngine.SceneManagement.SceneManager.LoadScene("SelectCharacters");
    }

    async public Task KickPlayer(string playerId){
        if(!ActiveSession.IsHost) return;
        await ActiveSession.AsHost().
        RemovePlayerAsync(playerId);
    }

    async public Task LeaveSession(){
        if(ActiveSession != null){
            try {
                await ActiveSession.LeaveAsync();
            }
            catch {
                //ignored as we are leaving the game
            }
            finally {
                ActiveSession = null;
            }

        }
    }

}
