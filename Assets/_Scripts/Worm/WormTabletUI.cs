using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Wastepunk.Creatures;
using System.Collections.Generic;

namespace Wastepunk.UI
{
    public class WormTabletUI : MonoBehaviour
    {
        #region Variables

        [SerializeField] private WormController monsterController;
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private TextMeshProUGUI recipeNameText;
        [SerializeField] private Image[] recipeIconSlots;

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
            {
                ClearIconSlots();
                if (recipeNameText != null) recipeNameText.text = "";
                return;
            }

            if (recipeNameText != null)
                recipeNameText.text = monsterController.DesiredRecipe.RecipeName;

            UpdateIconSlots();
        }

        private void UpdateIconSlots()
        {
            if (recipeIconSlots == null || recipeIconSlots.Length == 0) return;

            List<Sprite> icons = monsterController.DesiredRecipe.Steps;

            for (int i = 0; i < recipeIconSlots.Length; i++)
            {
                if (icons != null && i < icons.Count)
                {
                    recipeIconSlots[i].sprite = icons[i];
                    recipeIconSlots[i].gameObject.SetActive(true);
                }
                else
                {
                    recipeIconSlots[i].gameObject.SetActive(false);
                }
            }
        }

        private void ClearIconSlots()
        {
            if (recipeIconSlots == null) return;

            foreach (var slot in recipeIconSlots)
            {
                slot.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}