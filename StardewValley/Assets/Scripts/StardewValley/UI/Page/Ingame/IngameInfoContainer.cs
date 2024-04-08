using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace WATP.UI
{
    public class IngameInfoContainer : UIElement
    {
        private Text dayTxt;
        private Text timeTxt;
        private Text moneyTxt;


        public override void Initialize(RectTransform rect)
        {
            base.Initialize(rect);

            dayTxt = rectTransform.RecursiveFindChild("Txt_Day").GetComponent<Text>();
            timeTxt = rectTransform.RecursiveFindChild("Txt_Time").GetComponent<Text>();
            moneyTxt = rectTransform.RecursiveFindChild("Txt_Money").GetComponent<Text>();

            Bind();
            OnDayUpdate(Root.State.day.Value);
            OnTimeUpdate(Root.State.time.Value);
            OnMoneyUpdate(Root.State.player.money.Value);
        }

        public override void Dispose()
        {
            UnBind();

            base.Dispose();
        }

        void Bind()
        {
            Root.State.day.onChange += OnDayUpdate;
            Root.State.time.onChange += OnTimeUpdate;
            Root.State.player.money.onChange += OnMoneyUpdate;
        }

        void UnBind()
        {
            Root.State.day.onChange -= OnDayUpdate;
            Root.State.time.onChange -= OnTimeUpdate;
            Root.State.player.money.onChange -= OnMoneyUpdate;
        }

        void OnDayUpdate(int day)
        {
            dayTxt.text = $"{MonthStr(Root.State.month.Value)}, {DayStr(day)}";

        }

        void OnTimeUpdate(int time)
        {
            timeTxt.text = TimeStr(time);
        }

        void OnMoneyUpdate(int money)
        {
            moneyTxt.text = $"{money}";
        }

        string MonthStr(int month)
        {
            if (month == 1)
                return "��";
            else if (month == 2)
                return "����";
            else if (month == 3)
                return "����";
            else if (month == 4)
                return "�ܿ�";
            else
                return "";
        }

        string DayStr(int day)
        {
            var dayofweek = Root.State.dayOfWeek.Value;
            if (dayofweek == DayOfWeek.Sunday)
                return $"��, {day}";
            else if (dayofweek == DayOfWeek.Monday)
                return $"��, {day}";
            else if (dayofweek == DayOfWeek.Tuesday)
                return $"ȭ, {day}";
            else if (dayofweek == DayOfWeek.Wednesday)
                return $"��, {day}";
            else if (dayofweek == DayOfWeek.Thursday)
                return $"��, {day}";
            else if (dayofweek == DayOfWeek.Friday)
                return $"��, {day}";
            else if (dayofweek == DayOfWeek.Saturday)
                return $"��, {day}";

            return "";
        }

        string TimeStr(int time)
        {
            if (time < 720)
                return $"{HourValue(time)}:{MinValue(time)}, ����";
            else if (time < 1440)
                return $"{HourValue(time)}:{MinValue(time)}, ����";
            else
                return $"{HourValue(time)}:{MinValue(time)}, ����";
        }

        string HourValue(int time)
        { 
            return string.Format("{0:D2}", (time / 60));
        }

        string MinValue(int time)
        {
            return string.Format("{0:D2}", (time % 60));
        }
    }
}
