using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectImages : MonoBehaviour {
    [SerializeField] private List<Texture2D> possibleClues;

    private RawImage clueImage;
    public int index;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        index = UnityEngine.Random.Range(0, possibleClues.Count - 1);
        clueImage = GetComponentInChildren<RawImage>();
        clueImage.texture = possibleClues[index];
    }

    public void NextClue() {
        index++;
        if(index >= possibleClues.Count) {
            index = 0;
        }
        clueImage.texture = possibleClues[index];
    }

    public void PrevClue() {
        index--;
        if(index < 0) {
            index = possibleClues.Count - 1;
        }
        clueImage.texture = possibleClues[index];
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
