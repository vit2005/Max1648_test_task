using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button endTurnBtn;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private GameObject enemyTurnVisualGameObject;

    private bool _isAuto = false;

    private void Start()
    {
        endTurnBtn.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        _isAuto = PlayerPrefs.GetInt("isAuto") == 1;

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        UnitManager.Instance.OnAllEnemiesDead += Instance_OnAllEnemiesDead;

        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void Instance_OnAllEnemiesDead(bool obj)
    {
        StartCoroutine(DelayedEnd());
    }

    private IEnumerator DelayedEnd()
    {
        yield return new WaitForSeconds(2f);
        EndGame();
    }

    public void EndGame()
    {
        SceneManager.LoadScene(0);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }


    private void UpdateTurnText()
    {
        turnNumberText.text = "TURN " + TurnSystem.Instance.GetTurnNumber();
    }

    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButtonVisibility()
    {
        endTurnBtn.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn() && !_isAuto);
    }


}
