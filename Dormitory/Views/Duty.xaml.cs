﻿using Dormitory.Models;
using Dormitory.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Dormitory.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Duty : Page
    {
        public Duty()
        {
            ViewModel = new DutyViewModel();
            this.InitializeComponent();
            ViewModel.init(App.account);
        }
        DutyViewModel ViewModel;

        private void HomeAppButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Info), "");
        }

        private void CheckAppButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Checkbook), "");
        }

        private void DutyAppButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private async void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            //总次数倒数之和
            double TotalCount = 0;
            double random = 0;
            //概率模型
            double[] pro = new double[ViewModel.countitems.Count];
            for (int i = 0; i < ViewModel.countitems.Count; i++)
            {
                double a = 1.0/ (double)ViewModel.countitems[i].count;
                TotalCount += a;
            }
            for (int i = 0; i < ViewModel.countitems.Count; i++)
            {
                double a = 1.0 / (double)ViewModel.countitems[i].count;
                if (i == 0)
                {
                    pro[i] = a / TotalCount;
                }
                else
                {
                    pro[i] = a / TotalCount + pro[i - 1];
                }
            }
            Random ro = new Random();
            random = ro.NextDouble();
            int no = 0;
            for (int i = 0; i < ViewModel.countitems.Count; i++)
            {
                if (random < pro[i])
                {
                    no = i;
                    break;
                }
            }
            while (ViewModel.state[no] == false)
            {
                ro = new Random();
                random = ro.NextDouble();
                no = 0;
                for (int i = 0; i < ViewModel.countitems.Count; i++)
                {
                    if (random < pro[i])
                    {
                        no = i;
                        for (int j = 0; i < ViewModel.state.Length; i++)
                        {
                            ViewModel.state[i] = false;
                        }
                        break;
                    }
                }
            }
            this.pict.Source = new BitmapImage(ViewModel.memberitems[no].pic);
            //(pict.Source as BitmapImage).UriSource = ViewModel.memberitems[no].pic;
            DateTime date = DateTime.Now;
            string note = Note.Text;
            var result = await HttpUtil.GetMemberNames(App.account);  //狗哥把这段改为直接从mem_List拿名字
            string name = (string)result["names"][no];
            DutyItem D = new DutyItem(no, name, date, note);
            ViewModel.dutyitems.Add(D);

            await HttpUtil.AddDuty(App.account, no, D);

        }

        private void checked_click(object sender, RoutedEventArgs e)
        {
            var currentitem = ((CheckBox)e.OriginalSource).DataContext as MemberItem;
            var currnetname = currentitem.name;
            for(var i = 0; i < ViewModel.memberitems.Count; i++)
            {
                if (ViewModel.memberitems[i].name == currnetname)
                {
                    ViewModel.state[i] = true;
                }
            }
        }

        private void unchecked_click(object sender, RoutedEventArgs e)
        {
            var currentitem = ((CheckBox)e.OriginalSource).DataContext as MemberItem;
            var currnetname = currentitem.name;
            for (var i = 0; i < ViewModel.memberitems.Count; i++)
            {
                if (ViewModel.memberitems[i].name == currnetname)
                {
                    ViewModel.state[i] = false;
                }
            }
        }

    }

   

}
