using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance = null;
    private int EnemyCounter = 0;
    private int TotalEnemyCounter = 3;
    [SerializeField] private GameObject player;

    private IEnumerator SpawnEnemy(int count)
    {
        while(EnemyCounter< count)
        { 
        yield return new WaitForSeconds(Random.Range(5,20));
        Create("Prefabs/Enemy");
        EnemyCounter++;
        }
    }
    void Awake()
    {
        Instance = this;
        StartGame();
    }
    

    //Некоторый метод, позволяющий запускать игру повторно
    void StartGame()
    {
        if(!player)
        {
            Create("Prefabs/Player");
        }

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected)
        {
            StartCoroutine(SpawnEnemy(TotalEnemyCounter));
        }
    }

    //Создание игрока (создается и удаляется через Photon
    void Create(string PrefabName)
    {
        float x = Random.Range(-10,10);
        float z = Random.Range(-10, 10);
        Vector3 pos = new Vector3(x, 5f ,z);
        GameObject temp = PhotonNetwork.Instantiate(PrefabName, pos , Quaternion.identity);
        //((MonoBehaviour)temp.GetComponent("FirstPersonController")).enabled = true;
        //(temp.transform.GetComponentInChildren<Camera>()).enabled = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        PhotonNetwork.LoadLevel(0);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Disconnect();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        if(PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartCoroutine(SpawnEnemy(TotalEnemyCounter - EnemyCounter));
        }
    }

    public void LeaveHand()
    {
        PhotonNetwork.LeaveRoom();
    }
}
