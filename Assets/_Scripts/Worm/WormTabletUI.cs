using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Wastepunk.Creatures;

namespace Wastepunk.UI
{
    public class WormTabletUI : MonoBehaviour
    {
        #region Variables

        [SerializeField] private WormController monsterController;
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private Image recipeImage;
        [SerializeField] private TextMeshProUGUI recipeNameText;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (monsterController != null)
                monsterController.OnMonsterStateChanged += UpdateUI;
        }

        private void OnDisable()
        {
            if (monsterController != null)
                monsterController.OnMonsterStateChanged -= UpdateUI;
        }

        private void Start() => UpdateUI();

        #endregion

        #region Logic

        private void UpdateUI()
        {
            if (monsterController == null)
                return;

            if (hungerSlider != null)
                hungerSlider.value = monsterController.HungerRatio;

            if (monsterController.DesiredRecipe == null)
                return;

            if (recipeImage != null)
                recipeImage.sprite = monsterController.DesiredRecipe.RecipeIcon;

            if (recipeNameText != null)
                recipeNameText.text = monsterController.DesiredRecipe.RecipeName;
        }

        #endregion
    }
}
