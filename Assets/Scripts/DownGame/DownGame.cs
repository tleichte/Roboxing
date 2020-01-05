using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DownGame : MonoBehaviour
{

    public int MaxSpaces;

    public RectTransform TopNode;
    public RectTransform BottomNode;

    public RectTransform WireParent;
    public DownGameWire WirePrefab;

    private DownGameWire[] wires;

    private Player player;

    private int currWire;
    private bool playing;

    private float keyBuffer = 0.15f;

    public void Initialize(Player p) {
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
            wires[i].Initialize(targetNum, MaxSpaces);
        }

        wires[0].SetActive();

        playing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (playing) {

            if (wires[currWire].Position > -MaxSpaces && InputManager.GetKeyDelay(player.Player1, InputType.Left, keyBuffer)) { 
                wires[currWire].Position -= 1;
                AudioManager.Inst.PlayOneShot("DownLR");
            }
            if (wires[currWire].Position < MaxSpaces && InputManager.GetKeyDelay(player.Player1, InputType.Right, keyBuffer)) {
                wires[currWire].Position += 1;
                AudioManager.Inst.PlayOneShot("DownLR");
            }


            if (wires[currWire].IsAligned && InputManager.GetKeyDown(player.Player1, InputType.Confirm)) {
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
