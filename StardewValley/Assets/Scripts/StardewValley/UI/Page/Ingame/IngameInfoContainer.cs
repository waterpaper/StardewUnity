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
                return "봄";
            else if (month == 2)
                return "여름";
            else if (month == 3)
                return "가을";
            else if (month == 4)
                return "겨울";
            else
                return "";
        }

        string DayStr(int day)
        {
            var dayofweek = Root.State.dayOfWeek.Value;
            if (dayofweek == DayOfWeek.Sunday)
                return $"일, {day}";
            else if (dayofweek == DayOfWeek.Monday)
                return $"월, {day}";
            else if (dayofweek == DayOfWeek.Tuesday)
                return $"화, {day}";
            else if (dayofweek == DayOfWeek.Wednesday)
                return $"수, {day}";
            else if (dayofweek == DayOfWeek.Thursday)
                return $"목, {day}";
            else if (dayofweek == DayOfWeek.Friday)
                return $"금, {day}";
            else if (dayofweek == DayOfWeek.Saturday)
                return $"토, {day}";

            return "";
        }

        string TimeStr(int time)
        {
            if (time < 720)
                return $"{HourValue(time)}:{MinValue(time)}, 오전";
            else if (time < 1440)
                return $"{HourValue(time)}:{MinValue(time)}, 오후";
            else
                return $"{HourValue(time)}:{MinValue(time)}, 새벽";
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
