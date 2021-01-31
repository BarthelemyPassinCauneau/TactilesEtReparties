﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Voice.Unity; 

public class EleveExe : MonoBehaviourPun
{
    [SerializeField] TMP_Dropdown ChooseNote = null;
    [SerializeField] TMP_Text LabelNote = null;
    [SerializeField] Image image = null;
    [SerializeField] TMP_Text feedback = null;
    [SerializeField] Button confirm = null;
    [SerializeField] TMP_Text title = null;
    [SerializeField] VoiceConnection voiceConnection = null;
    string imagePath = "";
    string correctAnswer = "";

    void Start()
    {
        InitDropdown();
        title.text = "Kakoophonie - Eleve "+ PhotonNetwork.NickName;
        confirm.interactable = false;
    }

    void InitDropdown(){
        ChooseNote.ClearOptions();
        ChooseNote.options.Add (new TMP_Dropdown.OptionData() {text="Do"});
        ChooseNote.options.Add (new TMP_Dropdown.OptionData() {text="Re"});
        ChooseNote.options.Add (new TMP_Dropdown.OptionData() {text="Mi"});
        ChooseNote.options.Add (new TMP_Dropdown.OptionData() {text="Fa"});
        ChooseNote.options.Add (new TMP_Dropdown.OptionData() {text="Sol"});
        ChooseNote.options.Add (new TMP_Dropdown.OptionData() {text="La"});
        ChooseNote.options.Add (new TMP_Dropdown.OptionData() {text="Si"});
        LabelNote.text = ChooseNote.options[0].text;
        feedback.text = "En attente ...";
        feedback.color = Color.gray; 
    }

    [PunRPC]
    public void ReceiveExercise(string key, string note){
        confirm.interactable = true;
        feedback.text = "";
        imagePath = "Images/"+key+"/"+note;
        correctAnswer = note;
        image.sprite = Resources.Load<Sprite>(imagePath);
    }

    [PunRPC]
    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    public void OnLeftRoom() {
        PhotonNetwork.LoadLevel("Menu");
    }

    public void ConfirmAnswer(){
        confirm.interactable = false;
        if (ChooseNote.options[ChooseNote.value].text == correctAnswer){
            feedback.text = "Bonne réponse";
            feedback.color = Color.green; 
        } else {
            feedback.text = "Mauvaise réponse";
            feedback.color = Color.yellow;
        }
        //Photon, send my answer to professor
        photonView.RPC("ReceiveAnswer", RpcTarget.MasterClient, ChooseNote.options[ChooseNote.value].text);
    }

    public void RaiseHand() {
        photonView.RPC("StudentRaiseHand", RpcTarget.MasterClient);
        voiceConnection.PrimaryRecorder.TransmitEnabled = false;
    }

    [PunRPC]
    public void SpeakToClass() {
        voiceConnection.PrimaryRecorder.TransmitEnabled = !voiceConnection.PrimaryRecorder.TransmitEnabled;
    }
}
