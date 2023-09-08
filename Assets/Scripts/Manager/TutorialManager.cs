using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering.Universal;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager Instance;

    [Header("TraningEnemy")]
    [SerializeField] Enemy[] enemys;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    [Header("Mission")]
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private ParticleSystem completeParticle;
    private Animator missionPanelAnim;

    [Header("Script")]
    [SerializeField] ConfirmationPopupMenu confirmationPopupMenu;
    [SerializeField] private Player player;

    [Header("Camera")]
    [SerializeField] private Camera cameraUI;

    private void Awake() {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            Debug.LogWarning("Found more than one TutorialManager in the scene");
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        player = FindObjectOfType<Player>();
        missionPanelAnim = missionPanel.GetComponent<Animator>();

        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(cameraUI);
    }

    private void Start() {
        missionPanel.SetActive(false);

        PlayerInputControls.Instance.ChangeMapUI();
        confirmationPopupMenu.SelectConfirm();
        confirmationPopupMenu.ActivateMenu("튜토리얼을 진행할까요?",
        () =>
        {
            Debug.Log("Yes");
            confirmationPopupMenu.Escape_ConfirmMenu();
            GameManager.Instance.SceneChange("TutorialScene", () => {}, () =>{});
            SceneManager.sceneLoaded += OnTutoralSceneLoaded;
        },
        () =>
        {
            Debug.Log("No");
            GameManager.Instance.playerData.doTutorial = false;
            DataPersistenceManager.Instance.SaveGame();
            confirmationPopupMenu.Escape_ConfirmMenu();
            PlayerInputControls.Instance.ChangeMapPlayer();
            Destroy(gameObject);
        });

        if(GameManager.Instance.playerData.doTutorial){
            

        }
    }

    public void OnTutoralSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Tutoral SceneLoaded");
        GameManager.Instance.isInDungeon = true;
        GameManager.Instance.PlayerStartPosition();

        enemys = FindObjectsOfType<Enemy>(true);
        StartCoroutine(Tutorial());

        SceneManager.sceneLoaded -= OnTutoralSceneLoaded;
    }

    private void Update() {
        
    }
    
    IEnumerator Tutorial(){
        // 움직이기
        DialogueManager.Instance.EnterDialogueMode(inkJSON);
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogOpened());
        missionPanel.SetActive(true);

        float moveAmount = 0;
        while(true){
            moveAmount += PlayerInputControls.Instance.hvInputVec.magnitude * Time.deltaTime;

            if(moveAmount >= 3){
                moveAmount = 3;
                completeParticle.Play();
                missionPanelAnim.Play("MissionPanelBounce", 0, 0);
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SparkleImpact01);
                break;
            }

            if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
                missionText.text = string.Format("WASD로 움직이세요! \n{0:F2} / {1}", moveAmount, 3);
            else
                missionText.text = string.Format("왼쪽 스틱(<sprite=28>)로 움직이세요! \n {0:F2} / {1}", moveAmount, 3);

            yield return null;
        }

        //회피
        DialogueManager.Instance.ResumeDialogueMode();
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogOpened());
        missionPanelAnim.Play("MissionPanelBounce", 0, 0);

        player.dodgeCount = 0;
        while(true){
            if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
                missionText.text = string.Format("SPACE를 눌러 회피하세요! \n{0} / {1}", player.dodgeCount, 3);
            else
                missionText.text = string.Format("<sprite=21>버튼을 눌러 회피하세요!\n {0} / {1}", player.dodgeCount, 3);

            if(player.dodgeCount > 2){
                completeParticle.Play();
                missionPanelAnim.Play("MissionPanelBounce", 0, 0);
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SparkleImpact01);
                yield return new WaitForSeconds(1f);
                break;
            }
            yield return null;
        }


        //일반 공격
        DialogueManager.Instance.ResumeDialogueMode();
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogOpened());
        missionPanelAnim.Play("MissionPanelBounce", 0, 0);

        player.attackCount = 0;
        while (true)
        {
            if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
                missionText.text = string.Format("마우스 좌클릭으로 공격하세요!\n{0} / {1}", player.attackCount, 5);
            else
                missionText.text = string.Format("<sprite=23>버튼을 눌러 공격하세요!\n{0} / {1}", player.attackCount, 5);
            
            if(player.attackCount > 4){
                completeParticle.Play();
                missionPanelAnim.Play("MissionPanelBounce", 0, 0);
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SparkleImpact01);
                yield return new WaitForSeconds(1f);
                break;
            }
            yield return null;
        }

        // 강공격
        DialogueManager.Instance.ResumeDialogueMode();
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogOpened());
        while (true)
        {
            
            if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
                missionText.text = string.Format("마우스 우클릭으로 공격하세요!\n{0} / {1}", player.strongAttackCount, 2);
            else
                missionText.text = string.Format("<sprite=22>버튼을 눌러 공격하세요!\n{0} / {1}", player.strongAttackCount, 2);

            if (player.strongAttackCount > 1)
            {
                completeParticle.Play();
                missionPanelAnim.Play("MissionPanelBounce", 0, 0);
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SparkleImpact01);
                yield return new WaitForSeconds(1f);
                break;
            }
            yield return null;
        }

        // 체력 회복
        DialogueManager.Instance.ResumeDialogueMode();
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogOpened());
        player.curHealth = 50;
        while (true)
        {
            if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
                missionText.text = string.Format("키보드 R키를 눌러 회복하세요.\n{0:F0} / {1}", player.curHealth, player.maxHealth);
            else
                missionText.text = string.Format("<sprite=20>버튼을 눌러 회복하세요.\n{0:F0} / {1}", player.curHealth, player.maxHealth);

            if (player.curHealth == player.maxHealth)
            {
                completeParticle.Play();
                missionPanelAnim.Play("MissionPanelBounce", 0, 0);
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SparkleImpact01);
                yield return new WaitForSeconds(1f);
                break;
            }
            yield return null;
        }

        // 경직 시키기
        enemys[0].gameObject.SetActive(true);
        enemys[0].HpFull = true;
        enemys[0].isTraining = true;
        DialogueManager.Instance.ResumeDialogueMode();
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogOpened());
        missionPanelAnim.Play("MissionPanelBounce", 0, 0);
        

        if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
            missionText.text = "일반공격(<sprite=42>)으로 적이 경직할때까지 공격하세요!";
        else
            missionText.text = "일반공격(<sprite=23>)으로 적이 경직할때까지 공격하세요!";

        while (true)
        {

            if (enemys[0].isStaggering)
            {
                completeParticle.Play();
                missionPanelAnim.Play("MissionPanelBounce", 0, 0);
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SparkleImpact01);
                yield return new WaitForSeconds(2f);
                break;
            }
            yield return null;
        }

        // 강한 경직 시키기
        DialogueManager.Instance.ResumeDialogueMode();
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogOpened());
        missionPanelAnim.Play("MissionPanelBounce", 0, 0);
        
        if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
            missionText.text = "강공격(<sprite=43>)으로 적을 경직시키세요!";
        else
            missionText.text = "강공격(<sprite=22>으로 적을 경직시키세요!";

        while (true)
        {
            if (enemys[0].isStaggering && enemys[0].anim.speed <= 0.5f)
            {
                completeParticle.Play();
                missionPanelAnim.Play("MissionPanelBounce", 0, 0);
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SparkleImpact01);
                yield return new WaitForSeconds(2f);
                break;
            }
            yield return null;
        }

        // 마나 채우기
        DialogueManager.Instance.ResumeDialogueMode();
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogOpened());
        missionPanelAnim.Play("MissionPanelBounce", 0, 0);

        player.curMP = 0;

        while (true)
        {
            missionText.text = string.Format("마나를 전부 회복할때까지 적을 공격하세요!\n{0:F0} / {1}", player.curMP, player.maxMP);

            if (player.curMP >= player.maxMP)
            {
                completeParticle.Play();
                missionPanelAnim.Play("MissionPanelBounce", 0, 0);
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SparkleImpact01);
                break;
            }
            yield return null;
        }

        // 스킬 사용
        DialogueManager.Instance.ResumeDialogueMode();
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogOpened());
        missionPanelAnim.Play("MissionPanelBounce", 0, 0);
        
        if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
            missionText.text = "Shift + <sprite=43> 스킬을 사용해보세요!";
        else
            missionText.text = "<sprite=25> + <sprite=22>로 스킬을 사용해보세요!";

        while (true)
        {
            if (player.prevAction == "Skill")
            {
                completeParticle.Play();
                missionPanelAnim.Play("MissionPanelBounce", 0, 0);
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SparkleImpact01);
                yield return new WaitForSeconds(1f);
                break;
            }
            yield return null;
        }

        // 락온
        DialogueManager.Instance.ResumeDialogueMode();
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogOpened());
        missionPanelAnim.Play("MissionPanelBounce", 0, 0);

        if(PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
            missionText.text = "마우스 휠을 클릭해 적을 락온하세요!";
        else
            missionText.text = "오른쪽 스틱(<sprite=29>)버튼을 눌라 적을 락온하세요!";

        while (true)
        {
            if (player.currentTarget != null)
            {
                completeParticle.Play();
                missionPanelAnim.Play("MissionPanelBounce", 0, 0);
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SparkleImpact01);
                yield return new WaitForSeconds(1f);
                break;
            }
            yield return null;
        }

        // 락온 변경
        DialogueManager.Instance.ResumeDialogueMode();
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogOpened());
        missionPanelAnim.Play("MissionPanelBounce", 0, 0);
        enemys[1].gameObject.SetActive(true);
        enemys[1].isTraining = true;
        enemys[1].HpFull = true;
        enemys[2].gameObject.SetActive(true);
        enemys[2].isTraining = true;
        enemys[2].HpFull = true;

        int targetChangeLeftCount = 0;
        int targetChangeRightCount = 0;

        while (true)
        {
            if (PlayerInputControls.Instance.playerInputAction.Player.LockOnLeft.WasPerformedThisFrame() 
            && player.currentTarget != null 
            && PlayerInputControls.Instance.playerInputAction.Player.LockOnLeft.ReadValue<float>() != 0)
            {
                targetChangeLeftCount++;
            }

            if (PlayerInputControls.Instance.playerInputAction.Player.LockOnRight.WasPerformedThisFrame() 
            && player.currentTarget != null 
            && PlayerInputControls.Instance.playerInputAction.Player.LockOnRight.ReadValue<float>() != 0)
            {
                targetChangeRightCount++;
            }

            if (PlayerInputControls.Instance.controlType == PlayerInputControls.ControlType.KeyboardMouse)
                missionText.text = string.Format("마우스 휠을 스크롤해 타겟을 변경하세요!\nLEFT: {0} / {1}  \nRIGHT: {2} / {3}"
                , targetChangeLeftCount, 2, targetChangeRightCount, 2);
            else
                missionText.text = string.Format("<sprite=24> 또는 <sprite=26> 버튼을 눌러 타겟을 변경하세요!\nLEFT: {0} / {1}  \nRIGHT: {2} / {3}"
                , targetChangeLeftCount, 2, targetChangeRightCount, 2);

            if(targetChangeLeftCount > 1 && targetChangeRightCount > 1){
                completeParticle.Play();
                missionPanelAnim.Play("MissionPanelBounce", 0, 0);
                SoundManager.Instance.PlayUISound(SoundManager.UISFXType.SparkleImpact01);
                yield return new WaitForSeconds(2f);
                break;
            }

            yield return null;
        }

        DialogueManager.Instance.ResumeDialogueMode();

        yield return new WaitUntil(() => !DialogueManager.Instance.dialogueIsPlaying);
        GameManager.Instance.isInDungeon = false;
        GameManager.Instance.playerData.doTutorial = false;
        player.ReleaseLockOn();
        missionPanel.SetActive(false);

        GameManager.Instance.SceneChange("StartMap", () =>{
            DataPersistenceManager.Instance.SaveGame();
            GameManager.Instance.PlayerStartPosition();
        },
        () => {
            GameManager.Instance.PlayerStartPosition();
        }
        );
    }
}
