using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {
    public GameObject startingPoint;

    public Rope rope;

    public CameraFollow cameraFollow;

    Gnome currentGnome;

    public GameObject gnomePrefab;

    public RectTransform mainManu;
    public RectTransform gameplayMenu;
    public RectTransform gameOverMenu;

    public bool gnomeIvincible { get; set; }

    public float delayAfterDeath = 1.0f;

    public AudioClip gnomeDiedSound;
    public AudioClip gameOverSound;

    private void Start() {
        Reset();
    }

    public void Reset() {
        if(mainManu) {
            mainManu.gameObject.SetActive(false);
        }

        if(gameOverMenu) {
            gameOverMenu.gameObject.SetActive(false);
        }

        if(gameplayMenu) {
            gameplayMenu.gameObject.SetActive(true);
        }

        var resetObjects = FindObjectsOfType<Resettable>();

        foreach(Resettable r in resetObjects) {
            r.Reset();
        }

        CreateNewGnome();

        Time.timeScale = 1.0f;
    }

    public void CreateNewGnome() {
        RemoveGnome();

        GameObject newGnome = (GameObject)Instantiate(gnomePrefab, startingPoint.transform.position, Quaternion.identity);

        currentGnome = newGnome.GetComponent<Gnome>();

        rope.gameObject.SetActive(true);
        rope.connectedObject = currentGnome.ropeBody;
        rope.ResetLength();

        cameraFollow.target = currentGnome.cameraFollowTarget;
    }

    public void RemoveGnome() {
        if(gnomeIvincible) {
            return;
        }

        rope.gameObject.SetActive(false);

        cameraFollow.target = null;

        if(currentGnome != null) {
            currentGnome.holdingTreasure = false;
            currentGnome.gameObject.tag = "Untagged";

            foreach(Transform child in currentGnome.transform) {
                child.gameObject.tag = "Untagged";
            }

            currentGnome = null;
        }
    }

    void KillGnome(Gnome.DamageType damageType) {
        var audio = GetComponent<AudioSource>();
        if(audio) {
            audio.PlayOneShot(this.gnomeDiedSound);
        }

        currentGnome.ShowDamageEffect(damageType);

        if(gnomeIvincible == false) {
            currentGnome.DestroyGnome(damageType);

            RemoveGnome();

            StartCoroutine(ResetAfterDelay());
        }
    }

    IEnumerator ResetAfterDelay() {
        yield return new WaitForSeconds(delayAfterDeath);

        Reset();
    }

    public void TrapTouched() {
        KillGnome(Gnome.DamageType.Slicing);
    }

    public void FireTrapTouched() {
        KillGnome(Gnome.DamageType.Burning);
    }

    public void TreasureCollected() {
        currentGnome.holdingTreasure = true;
    }

    public void ExitReached() {
        if(currentGnome != null && currentGnome.holdingTreasure == true) {
            var audio = GetComponent<AudioSource>();
            if(audio) {
                audio.PlayOneShot(this.gameOverSound);
            }

            Time.timeScale = 0.0f;

            if(gameOverMenu) {
                gameOverMenu.gameObject.SetActive(true);
            }

            if(gameplayMenu) {
                gameplayMenu.gameObject.SetActive(false);
            }
        }
    }

    public void SetPause(bool paused) {
        if(paused) {
            Time.timeScale = 0.0f;
            mainManu.gameObject.SetActive(true);
            gameplayMenu.gameObject.SetActive(false);
        }
        else {
            Time.timeScale = 1.0f;
            mainManu.gameObject.SetActive(false);
            gameplayMenu.gameObject.SetActive(true);
        }
    }

    public void RestartGame() {
        Destroy(currentGnome.gameObject);
        currentGnome = null;

        Reset();
    }
}
