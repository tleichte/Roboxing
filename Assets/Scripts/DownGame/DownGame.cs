using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DownGame : MonoBehaviour
{
    public RectTransform TopNode;
    public RectTransform BottomNode;

    public RectTransform WireParent;
    public DownGameWire WirePrefab;

    private DownGameWire[] wires;

    private Player player;

    private int currWire;
    private bool playing;

    private float keyBuffer = 0.2f;
    private float time;

    //void Start() {
    //    GameManager.Inst.OnGameOver += OnGameOver;

    //}

    //void OnDestroy() {
    //    GameManager.Inst.OnGameOver -= OnGameOver;
    //}

    //void OnGameOver(GameOverResult _, GameOverReason __) {
    //    if (playing) OnSleep();
    //}


    public void Initialize(Player p) {

        currWire = 0;
        time = 0;
        player = p;
        foreach (RectTransform wire in WireParent)
            Destroy(wire.gameObject);
        wires = new DownGameWire[GameManager.Inst.GetNumWires(p.Player1)];

        int targetNum = Random.Range(-2, 2);

        TopNode.anchoredPosition = new Vector2(40 * targetNum, 0);
        BottomNode.anchoredPosition = new Vector2(40 * targetNum, 0);

        for (int i = 0; i < wires.Length; i++) {
            wires[i] = Instantiate(WirePrefab, WireParent);
            wires[i].Initialize(targetNum);
        }

        wires[0].SetActive();

        playing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (playing) {
            if (Input.GetKey(HGDCabKeys.Of(player.PlayerPos).JoyLeft) && wires[currWire].Position > -5) {
                if (time <= 0) {
                    wires[currWire].Position -= 1;
                    time = keyBuffer;
                    AudioManager.Inst.PlayOneShot("DownLR");
                }
                time -= Time.deltaTime;
            }
            else if (Input.GetKey(HGDCabKeys.Of(player.PlayerPos).JoyRight) && wires[currWire].Position < 5) {
                if (time <= 0) {
                    wires[currWire].Position += 1;
                    time = keyBuffer;
                    AudioManager.Inst.PlayOneShot("DownLR");
                }
                time -= Time.deltaTime;
            }
            else
                time = 0;


            if (Input.GetKeyDown(HGDCabKeys.Of(player.PlayerPos).Top1) && wires[currWire].IsAligned) {
                AudioManager.Inst.PlayOneShot("DownCorrect");
                wires[currWire].Confirmed = true;
                currWire++;
                if (currWire < wires.Length)
                    wires[currWire].SetActive();
            }

            if (currWire >= wires.Length) {
                FinishGame();
            }
        }
    }


    private void FinishGame() {
        AudioManager.Inst.PlayOneShot("DownDone");
        player.Recover();
        playing = false;
    }


    public void OnSleep() {
        playing = false;
    }
}
