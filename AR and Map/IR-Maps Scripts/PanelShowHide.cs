using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Try : MonoBehaviour
{
    public Button IdeaButton,CaptureButton,BackButton, exit;
    public GameObject IdeaButtonPanel, juanitoPanel, imuscathedralmarkerPanel, imuscathedralheritagebellsPanel, imuscathedralPanel, imusplazacarabaoPanel, imusplazaPanel, imusplazageneraltopacioPanel, pillarlodgeno3Panel, tulaytomasmascardoPanel,imushistoricalmarkerPanel, labanansaimusPanel, bridgeofisabelPanel, battleofalapanPanel;

    public void ideabutton() {
        IdeaButtonPanel.SetActive(true);
        IdeaButton.gameObject.SetActive(false);
        CaptureButton.gameObject.SetActive(false);
        BackButton.gameObject.SetActive(false);
        juanitoPanel.SetActive(false);
        imuscathedralmarkerPanel.SetActive(false);
        imuscathedralheritagebellsPanel.SetActive(false);
        imuscathedralPanel.SetActive(false);
        imusplazacarabaoPanel.SetActive(false);
        imusplazaPanel.SetActive(false);
        imusplazageneraltopacioPanel.SetActive(false);
        pillarlodgeno3Panel.SetActive(false);
        tulaytomasmascardoPanel.SetActive(false);
        imushistoricalmarkerPanel.SetActive(false);
        labanansaimusPanel.SetActive(false);
        bridgeofisabelPanel.SetActive(false);
        battleofalapanPanel.SetActive(false);

        
    }
    public void Exit()
    {
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);

    }

//======================================================================================================================
    public void Juanitobtn()
    {
        juanitoPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void JuanitoPanelRemove(){
        juanitoPanel.SetActive(false);
    }

//======================================================================================================================

    public void imuscathedralmarker_btn()
    {
        imuscathedralmarkerPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void ImusCathedralMarkerPanelRemove(){
        imuscathedralmarkerPanel.SetActive(false);
    }

//======================================================================================================================

    public void imuscathedralheritagebells_btn()
    {
        imuscathedralheritagebellsPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void ImusCathedralHeritageBellsPanelRemove(){
        imuscathedralheritagebellsPanel.SetActive(false);
    }

//======================================================================================================================

    public void imuscathedral_btn()
    {
        imuscathedralPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void ImusCathedralPanelRemove(){
        imuscathedralPanel.SetActive(false);
    }

//======================================================================================================================
    public void imusplazacarabao_btn()
    {
        imusplazacarabaoPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void ImusPlazaCarabaoPanelRemove(){
        imusplazacarabaoPanel.SetActive(false);
    }

//======================================================================================================================
    public void imusplaza_btn()
    {
        imusplazaPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void ImusPlazaPanelRemove(){
        imusplazaPanel.SetActive(false);
    }

//======================================================================================================================
    public void imusplazageneraltopacio_btn()
    {
        imusplazageneraltopacioPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void ImusPlazaGeneralTopacioPanelRemove(){
        imusplazageneraltopacioPanel.SetActive(false);
    }
    
//======================================================================================================================
    public void pillarlodgeno3_btn()
    {
        pillarlodgeno3Panel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void PillarLodgeNo3PanelRemove(){
        pillarlodgeno3Panel.SetActive(false);
    }

//======================================================================================================================
    public void tulaytomasmascardo_btn()
    {
        tulaytomasmascardoPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void TulayTomasMascardoPanelRemove(){
        tulaytomasmascardoPanel.SetActive(false);
    }

//======================================================================================================================
    public void imushistoricalmarker_btn()
    {
        imushistoricalmarkerPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void ImusHistoricalMarkerPanelRemove(){
        imushistoricalmarkerPanel.SetActive(false);
    }

//======================================================================================================================
    public void labanansaimus_btn()
    {
        labanansaimusPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void LabananSaImusPanelRemove(){
        labanansaimusPanel.SetActive(false);
    }

//======================================================================================================================
    public void bridgeofisabel_btn()
    {
        bridgeofisabelPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void BridgeOfIsabelPanelRemove(){
        bridgeofisabelPanel.SetActive(false);
    }

//======================================================================================================================
    public void battleofalapan_btn()
    {
        battleofalapanPanel.SetActive(true);
        IdeaButtonPanel.SetActive(false);
        IdeaButton.gameObject.SetActive(true);
        CaptureButton.gameObject.SetActive(true);
        BackButton.gameObject.SetActive(true);
    }
    public void BattleOfAlapanPanelRemove(){
        battleofalapanPanel.SetActive(false);
    }

}
