using UnityEngine;
using Photon.Pun;

public class MiniMap : MonoBehaviourPun
{
    [SerializeField] private Transform _player;
    private Camera _mapCam;
    [SerializeField] private Shader _replShader; //шейдер, кот.будем накладывать на камеру, чтобы избавиться от многих ненужных эффектов
    [SerializeField] private Vector3 newPosition; //позиция плеера, промежуточный вектор

    [SerializeField] private PhotonView _photon;

    void Start()
    {
        _photon = FindObjectOfType<PhotonView>();
        _mapCam = GetComponent<Camera>();

        if (_photon.IsMine)
        {
            _mapCam.enabled = true;
            _player = FindObjectOfType<PhotonView>().transform;
        }
        else
        {
            _mapCam.enabled = false;
        }

        _replShader = Shader.Find("Toon/Basic");

        if(_replShader)
        {
            _mapCam.SetReplacementShader(_replShader, "RenderType"); //метод камеры, который меняет шейдер
        }
    }

    private void OnDisable()
    {
        _mapCam.ResetReplacementShader();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!_photon.IsMine && PhotonNetwork.IsConnected == true){ return; }

        if (_player)
        {
            newPosition = _player.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
            transform.rotation = Quaternion.Euler(90f, _player.eulerAngles.y, 0f);
        }
    }
}
