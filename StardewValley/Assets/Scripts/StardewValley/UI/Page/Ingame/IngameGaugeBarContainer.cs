using System;
using UnityEngine;
using UnityEngine.UI;

namespace WATP.UI
{
    public class IngameGaugeBarContainer : UIElement
    {
        private ProgressBarFill hpBar;
        private ProgressBarFill energyBar;


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            hpBar = rectTransform.RecursiveFindChild("HpBar").GetComponent<ProgressBarFill>();
            energyBar = rectTransform.RecursiveFindChild("EnergyBar").GetComponent<ProgressBarFill>();

            Bind();
        }

        public override void Dispose()
        {
            UnBind();

            base.Dispose();
        }

        public void Init()
        {
            hpBar.gameObject.SetActive(false);
        }


        void Bind()
        {
            Root.State.player.hp.onChange += OnHpChange;
            Root.State.player.actingPower.onChange += OnEnergyChange;

            Root.State.player.maxHp.onChange += OnHpChange;
            Root.State.player.actingPowerMax.onChange += OnEnergyChange;
        }

        void UnBind()
        {

        }

        void OnHpChange(int hp)
        {
            hpBar.Progress(1.0f * Root.State.player.hp.Value / Root.State.player.maxHp.Value);
        }


        void OnEnergyChange(int energy)
        {
            energyBar.Progress(1.0f * Root.State.player.actingPower.Value / Root.State.player.actingPowerMax.Value);
        }
    }
}
