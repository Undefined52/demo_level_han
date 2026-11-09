using UnityEngine;

public class CheckGuess : MonoBehaviour {
    [SerializeField] private int goodWhoIndex;
    [SerializeField] private int goodWhatIndex;
    [SerializeField] private int goodWhereIndex;

    [SerializeField] private Transform winPOI;

    private bool _guessGood = true;
    private ParticleSystem system;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        system = GetComponentInChildren<ParticleSystem>();
    }

    public void CheckGuesses() {
        _guessGood = true;
        SelectImages[] guessPanels = GetComponentsInChildren<SelectImages>(true);
        foreach(SelectImages guess in guessPanels) {
            switch(guess.gameObject.name) {
            case "What":
                _guessGood = guess.index == goodWhatIndex ? _guessGood : false;
                break;
            case "Where":
                _guessGood = guess.index == goodWhereIndex ? _guessGood : false;
                break;
            case "Who":
                _guessGood = guess.index == goodWhoIndex ? _guessGood : false;
                break;
                
            }
        }
        playerController controller = gameObject.transform.parent.parent.parent.parent.GetComponentInChildren<playerController>();
        controller.OnPickup(null);

        if(_guessGood) {
            system.Play();
            SheepAI[] ais = gameObject.transform.parent.parent.parent.parent.GetComponentsInChildren<SheepAI>();
            foreach(SheepAI ai in ais) {
                ai.interestPoints.RemoveRange(0, ai.interestPoints.Count);
                ai.interestPoints.Add(winPOI);
                ai.maxWanderDist = 5f;
                ai.destArrivedDist = 1f;
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
