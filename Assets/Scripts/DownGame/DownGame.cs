using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DownGame : MonoBehaviour
{

    public GameObject RecoveredBG;
    public GameObject SleepBG;
    public GameObject GameBG;

    public RectTransform TopNode;
    public RectTransform BottomNode;

    public RectTransform WireParent;
    public DownGameWire WirePrefab;

    private DownGameWire[] wires;

    private Player player;

    private int currWire;
    private bool playing;


    void Start() {
        GameManager.Inst.OnGameOver += OnGameOver;

    }

    void OnDestroy() {
        GameManager.Inst.OnGameOver -= OnGameOver;
    }

    void OnGameOver(GameOverResult _, GameOverReason __) {
        if (playing) OnSleep();
    }


    public void Initialize(Player p) {

        GameBG.SetActive(true);
        RecoveredBG.SetActive(false);
        SleepBG.SetActive(false);

        currWire = 0;
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
            if (Input.GetKeyDown(HGDCabKeys.Of(player.PlayerPos).JoyLeft) && wires[currWire].Position > -5)
                wires[currWire].Position -= 1;
            if (Input.GetKeyDown(HGDCabKeys.Of(player.PlayerPos).JoyRight) && wires[currWire].Position < 5)
                wires[currWire].Position += 1;


            if (Input.GetKeyDown(HGDCabKeys.Of(player.PlayerPos).Top1) && wires[currWire].IsAligned) {
                wires[currWire].Confirmed = true;
                currWire++;
                if (currWire < wires.Length)
                    wires[currWire].SetActive();
            }

            if (currWire >= wires.Length) {
                player.Recover();
                playing = false;
            }
        }
    }

    public void OnSleep() {
        GameBG.SetActive(false);
        SleepBG.SetActive(true);
        playing = false;
    }
    public void OnRecover() {
        GameBG.SetActive(false);
        RecoveredBG.SetActive(true);
    }
}
