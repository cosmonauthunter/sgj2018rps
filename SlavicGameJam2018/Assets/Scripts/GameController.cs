﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameController : MonoBehaviour {

	[Header("Initial countdown of a timer")]
    [SerializeField]
    float initialCountdownTime;
    float currentCountdownTime;
    float elapsedTime = -1;

    [SerializeField]
    RectTransform timeGauge;

    [Header("Player stats gauges")]
    [SerializeField]
    RectTransform player1HPBar;
    [SerializeField]
    RectTransform player2HPBar;

    [SerializeField]
    RectTransform player1MPBar;
    [SerializeField]
    RectTransform player2MPBar;
    [Header("Physical Card representations")]
    [SerializeField]
    SpriteRenderer player1CardSprite;
    [SerializeField]
    SpriteRenderer player2CardSprite;

    [Header("Reference to KO screens etc.")]
    [SerializeField]
    GameObject vicoryScreen;
    [SerializeField]
    GameObject player1WinText;
    [SerializeField]
    GameObject player2WinText;
    //[SerializeField]
    //GameObject tieText;

	Animator player1CardAniamator;
	Animator player2CardAniamator;
	Animator player1HealthBarAnimator;
	Animator player2HealthBarAnimator;

	AudioSource audioData;

	[SerializeField]
	Camera MainCameraRef;

    float initialGaugeSize;

    public enum choice { none, rock, paper, scissors };

    //shitty state machine equivalent
    bool inputAllowed = true;
    bool timerEnabled = false;

    float tieCounter = 0;

    public Player player1 { get; set; }
    public Player player2 { get; set; }

    //transition
    Vector3 vel1, vel2, desScale1, desScale2;
    
    // Use this for initialization
    void Start() {
		player1CardAniamator = player1CardSprite.GetComponent<Animator>();
        player2CardAniamator = player2CardSprite.GetComponent<Animator>();

		player1HealthBarAnimator = player1HPBar.GetComponent<Animator>();
		player2HealthBarAnimator = player2HPBar.GetComponent<Animator>();

        
        player1 = new Player();
        player2 = new Player();
        elapsedTime = currentCountdownTime = initialCountdownTime;
		initialGaugeSize = 1;
        vel1 = vel2 = Vector3.zero;
		desScale1 = desScale2 = timeGauge.localScale = new Vector3(1, 1, 1);
    }

    public void Initialise() {
		
		MainCameraRef.GetComponent<CameraFilterPack_TV_VHS_Rewind>().enabled = false;
        timerEnabled = true;

    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (inputAllowed) {
            if (player1.cardSwitches > 0) {
				if (Input.GetKeyDown("q")) {
					player1CardAniamator.enabled = true;
					player1.SwitchToRock();

					//player1CardAniamator.CrossFade("cardFlip", 0f); // Play flip animation

                }
				if (Input.GetKeyDown("w")) {
					player1CardAniamator.enabled = true;
					player1.SwitchToPaper();

					//player1CardAniamator.CrossFade("cardFlip", 0f); // Play flip animation


                }
				if (Input.GetKeyDown("e")) {
					player1CardAniamator.enabled = true;
					player1.SwitchToScissors();

					//player1CardAniamator.CrossFade("cardFlip", 0f); // Play flip animation

                }
            }
            if (player2.cardSwitches > 0) {
				if (Input.GetKeyDown("i")) {
					player2CardAniamator.enabled = true;
					player2.SwitchToRock();


                }
				if (Input.GetKeyDown("o")) {
					player2CardAniamator.enabled = true;
					player2.SwitchToPaper();



                }
				if (Input.GetKeyDown("p")) {
					player2CardAniamator.enabled = true;
					player2.SwitchToScissors();

                }
            }
        }
		if (timerEnabled)
        {
            elapsedTime -= Time.deltaTime;
        }      
        if (elapsedTime <= 0) {
            inputAllowed = false;

            Clash();
            //THIS IS WHERE ALL THE MEAT HAPPENS

            //Play clash animation
			player1CardAniamator.Play("cardAttackPlayer1", 0, 0f); // Play attack animation
			player1CardAniamator.enabled = true;
			player2CardAniamator.Play("cardAttackPlayer2",0,0f); // Play attack animation
			player2CardAniamator.enabled = true;

            
			if (currentCountdownTime>0.9f)
			{
				currentCountdownTime *= 0.95f;
			}
            elapsedTime = currentCountdownTime;
            //Timer resets...
        }
        
        timeGauge.localScale = new Vector3(initialGaugeSize * elapsedTime / currentCountdownTime, 1, 1);

        player1HPBar.localScale = Vector3.SmoothDamp(player1HPBar.localScale, desScale1, ref vel1, 0.5f);



        
        player2HPBar.localScale = Vector3.SmoothDamp(player2HPBar.localScale, desScale2, ref vel2, 0.5f);


		player1MPBar.localScale = new Vector3(player1.cardSwitches / player1.cardSwitchesMax, 1, 1);      
		player2MPBar.localScale = new Vector3(player2.cardSwitches / player2.cardSwitchesMax, 1, 1);

    }

    void Clash() {

        if (player1.choice == choice.none && player2.choice == choice.none) {
            PlayerTie();
        }
        if (player1.choice == choice.none && player2.choice != choice.none) {
            PlayerOneGetHit();
        }
        if (player2.choice == choice.none && player1.choice != choice.none) {
            PlayerTwoGetHit();
        }
        if (player1.choice == choice.rock) {
            if (player2.choice == choice.rock) { PlayerTie(); }
            if (player2.choice == choice.paper) { PlayerOneGetHit(); }
            if (player2.choice == choice.scissors) { PlayerTwoGetHit(); }
        }
        if (player1.choice == choice.paper) {
            if (player2.choice == choice.rock) { PlayerTwoGetHit(); }
            if (player2.choice == choice.paper) { PlayerTie(); }
            if (player2.choice == choice.scissors) { PlayerOneGetHit(); }
        }
        if (player1.choice == choice.scissors) {
            if (player2.choice == choice.rock) { PlayerOneGetHit(); }
            if (player2.choice == choice.paper) { PlayerTwoGetHit(); }
            if (player2.choice == choice.scissors) { PlayerTie(); }
        }
    }

	void PlayerTwoGetHit()
    {
		//falshing damage anim
        player2HealthBarAnimator.enabled = true;

        tieCounter = 0;
        player2.hp -= 1;
        if (player2.hp == 0)
        {
            PlayerOneWin();
        }
        desScale2 = new Vector3(player2.hp / player2.hpMax, 1, 1);
        inputAllowed = true;
    }
    void PlayerOneGetHit()
    {
		//falshing damage anim
        player1HealthBarAnimator.enabled = true;

        tieCounter = 0;
        player1.hp -= 1;
        if (player1.hp == 0)
        {
            PlayerTwoWin();
        }
        desScale1 = new Vector3(player1.hp / player1.hpMax, 1, 1);
        inputAllowed = true;
    }
    void PlayerTie()
    {
        tieCounter++;
        if (tieCounter > 10)
        {
            GameTie();
        }
        inputAllowed = true;
    }

    void PlayerOneWin()
    {
        vicoryScreen.SetActive(true);

		MainCameraRef.GetComponent<CameraFilterPack_TV_VHS>().enabled = true;
        player1WinText.SetActive(true);
        timerEnabled = false;
    }
    void PlayerTwoWin()
    {
		MainCameraRef.GetComponent<CameraFilterPack_TV_VHS>().enabled = true;
        vicoryScreen.SetActive(true);
        player2WinText.SetActive(true);
        timerEnabled = false;
    }
    void GameTie()
    {
		MainCameraRef.GetComponent<CameraFilterPack_TV_VHS>().enabled = true;
        vicoryScreen.SetActive(true);
        timerEnabled = false;
    }

    public void RestartGame()
    {
		MainCameraRef.GetComponent<CameraFilterPack_TV_VHS>().enabled = false;
		MainCameraRef.GetComponent<CameraFilterPack_TV_VHS_Rewind>().enabled = false;

        player1 = new Player();
        player2 = new Player();
        tieCounter = 0;
        timerEnabled = true;
        currentCountdownTime = initialCountdownTime;
        vicoryScreen.SetActive(false);
        player1WinText.SetActive(false);
        player2WinText.SetActive(false);

        desScale1 = desScale2 = timeGauge.localScale = new Vector3(1, 1, 1);
        //tieText.SetActive(false);
    }

	public void FightAudio()
	{
		audioData = GetComponent<AudioSource>();
        audioData.Play(0);
	}
}
