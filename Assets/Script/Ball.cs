using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Ball : MonoBehaviour
{
    private const float g = 39.8f;
    private const float _v0 = 3.5f;
    private float _v;
    // private float _smax = g * _v0;
    private float _smax = 3f;
    private float _t = 0;
    private float yFall = 0;
    private float yJump =0;
    private float _s0 = 0;
    private int checkDestroy;
    private int countDestroy;
    private bool checkFurry = false;
    public State _currentState = State.Jump;
    private int _undestroyable = 1;
    [SerializeField] private GamePlay disks;
    [SerializeField] private Image uiFill;
    public enum State
    {
        Jump,Fall,Smash,Fury,Die
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case State.Jump:
                //LAM CHO QUA BONG NHAY LEN
                yJump = _s0 + _v * _t + 0.5f * g * _t * _t;
                transform.position = new Vector3(0, yJump, 2.5f);
                //LAM CHO QUA BONG BIEN DANG KHI NHAY LEN
                transform.localScale = new Vector3((float)(1 - 0.6 * _t), (float)(1 + 0.2 * _t),(float)(1 - 0.6 * _v0));
                //PHAT HIEN THOI DIEM ROI XUONG
                if(yJump > _smax)
                {
                    ChangeSate(State.Fall);
                    return;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    ChangeSate(State.Smash);
                    return;
                }
                
                break;
            case State.Fall:
                //LAM CHO QUA BONG ROI XUONG
                yFall = _s0 + 0.5f * -g * _t * _t;
                transform.position = new Vector3(0, yFall, 2.5f);
                //LAM CHO QUA BONG BIEN DANG KHI ROI
                transform.localScale = new Vector3(1f + 0.6f * _t,1f - 0.2f * _t,1f);
                //PHAT HIEN THOI DIEM NHAY LEN
                if (yFall < disks.DiskList[0].transform.position.y + 1f)
                {
                    ChangeSate(State.Jump);
                    return;
                }

                if (Input.GetMouseButton(0))
                {
                    ChangeSate(State.Smash);
                    return;
                }
                
                break;
            case State.Smash:
                if (Input.GetMouseButton(0))
                {
                    float time = 0;
                    time += Time.deltaTime;
                    int count = 0;
                    transform.position -= new Vector3(0, 0.5f, 0)*0.5f;
                    if (transform.position.y < disks.DiskList[0].transform.position.y+0.5f)
                    {
                        Destroy(disks.DiskList[0]);
                        disks.DiskList.Remove(disks.DiskList[0]);
                        count++;
                        countDestroy++;
                        uiFill.fillAmount += 3f*time;
                    }
                    Debug.Log(countDestroy);
                    _smax -= count * 1f;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    ChangeSate(State.Fall);
                    return;
                }

                if (uiFill.fillAmount==1)
                {
                    checkFurry = true;
                }
                break;
            case State.Fury:
                break;
            case State.Die:
                gameObject.SetActive(false);
                disks.ChangeState(GamePlay.GameStates.Lose);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _t += Time.deltaTime;
    }
    private void ChangeSate(State state)
    {
        // Debug.Log(_currentState);
        if (state == _currentState) return;
        _currentState = state;
        switch (state)
        {
            case State.Jump:
                _s0 = disks.DiskList[0].transform.position.y+1f;
                _v = _v0;
                _t = 0;
                uiFill.fillAmount -= 0.1f;
                break;
            case State.Fall:
                _s0 = transform.position.y;
                _v = 0;
                _t = 0;
                uiFill.fillAmount -= 0.1f;
                break;
            case State.Smash:
                uiFill.fillAmount = 0;
                break;
            case State.Fury:
                break;
            case State.Die:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(_currentState == State.Smash && other.gameObject.CompareTag("Black_Piece") && checkFurry == false)
        {
            if (_undestroyable == 1)
            {
                var scaleSequence = DOTween.Sequence();
                scaleSequence.Append(gameObject.transform.DOScaleZ(2f, 3f))
                    .Append(gameObject.transform.DOScaleZ(2f, 5f));
                ChangeSate(State.Fall);
                _undestroyable--;
            }
            else
            {
                ChangeSate(State.Die);
            }
        }
        else if (_currentState == State.Smash && other.gameObject.CompareTag("Win_Piece"))
        {
            disks.ChangeState(GamePlay.GameStates.Win);
        }
    }

    private void Fury()
    {
        float time = 0;
        time += Time.deltaTime;
        uiFill.fillAmount -= 10f*time;
    }
}  