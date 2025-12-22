using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Work.Code.Supply;

namespace Work.Code.UI
{
    public class FoodMakeButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private Tooltip tooltip;
        [SerializeField] private Color disabledColor;
        
        private static readonly string MINERAL_FORMAT = "<color=#00ACFF>{0}</color> Minerals.";
        private static readonly string GAS_FORMAT = "<color=#3BEA60>{0}</color> Gas.";
        private static readonly string DEPENDENCY_FORMAT_NO_COMMA = "<color=#AC0000>{0}</color>.";
        private static readonly string DEPENDENCY_FORMAT_COMMA = "<color=#AC0000>{0}</color>,";
        
        public void EnableFor()
        {
            
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(tooltip != null )
                Invoke(nameof(ShowTooltip), tooltip.HoverDelay);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CancelInvoke();
            tooltip?.Hide();
        }
        
        private void ShowTooltip()
        {
            tooltip.Show();
        }
        
        // private string GetTooltipText(FoodData command)
        // {
        //     StringBuilder tooltipBuilder = new StringBuilder();
        //     tooltipBuilder.Append($"{command.Name}\n");
        //
        //     SupplyCostSO cost = null;
        //     if (command is BuildUnitCommandSO buildUnitCommand)
        //     {
        //         cost = buildUnitCommand.Unit.Cost;
        //     }else if (command is ConstructBuildingCommandSO constructBuildingCommand)
        //     {
        //         cost = constructBuildingCommand.BuildingData.Cost;
        //     }
        //
        //     if (cost != null)
        //     {
        //         if (cost.Minerals > 0)
        //             tooltipBuilder.Append(string.Format(MINERAL_FORMAT, cost.Minerals));
        //         if(cost.Gas > 0)
        //             tooltipBuilder.Append(string.Format(GAS_FORMAT, cost.Gas));
        //     }
        //
        //     if (command is IUnlockableCommand unlockableCommand
        //         && command.IsLocked(new CommandContext(uiOwner, null, new RaycastHit())))
        //     {
        //         UnlockableSO[] dependencies = unlockableCommand.GetUnMetDependencies(uiOwner);
        //
        //         if (dependencies.Length > 0)
        //         {
        //             tooltipBuilder.AppendLine();
        //             tooltipBuilder.AppendLine("Requires: ");
        //         }
        //
        //         for (int i = 0; i < dependencies.Length; i++)
        //         {
        //             tooltipBuilder.Append(i == dependencies.Length - 1
        //                 ? string.Format(DEPENDENCY_FORMAT_NO_COMMA, dependencies[i].Name)
        //                 : string.Format(DEPENDENCY_FORMAT_COMMA, dependencies[i].Name));
        //         }
        //
        //         // var line = dependencies.Select(dependency =>
        //         //     string.Format(DEPENDENCY_FORMAT_NO_COMMA, dependency.Name));
        //         // tooltipBuilder.Append(string.Join(",", line));
        //     }
        //
        //     return tooltipBuilder.ToString();
        // }
    }
}