using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OICPOSレジ_2C29KS.Models
{
    public class DrawerCashManager
    {
        // Encapsulation: အပြင်ကနေ တိုက်ရိုက်ပြင်လို့မရအောင် private သတ်မှတ်ထားသည်
        private int _count10000 = 5;
        private int _count5000 = 10;
        private int _count1000 = 20;
        private int _count500 = 30;
        private int _count100 = 50;
        private int _count50 = 50;
        private int _count10 = 100;

        // အပြင်ကနေ ဖတ်လို့ရအောင် Properties များ
        public int Count10000 => _count10000;
        public int Count5000 => _count5000;
        public int Count1000 => _count1000;
        public int Count500 => _count500;
        public int Count100 => _count100;
        public int Count50 => _count50;
        public int Count10 => _count10;

      
        public void ProcessCashTransaction(int paid, int change)
        {
            
            AddCash(paid);
           
            WithdrawCash(change);
        }

        private void AddCash(int amount)
        {
            _count10000 += amount / 10000; amount %= 10000;
            _count5000 += amount / 5000; amount %= 5000;
            _count1000 += amount / 1000; amount %= 1000;
            _count500 += amount / 500; amount %= 500;
            _count100 += amount / 100; amount %= 100;
            _count50 += amount / 50; amount %= 50;
            _count10 += amount / 10;
        }

        private void WithdrawCash(int amount)
        {
            _count10000 -= amount / 10000; amount %= 10000;
            _count5000 -= amount / 5000; amount %= 5000;
            _count1000 -= amount / 1000; amount %= 1000;
            _count500 -= amount / 500; amount %= 500;
            _count100 -= amount / 100; amount %= 100;
            _count50 -= amount / 50; amount %= 50;
            _count10 -= amount / 10;
        }

       
        public bool Exchange500To100()
        {
            if (_count500 >= 1)
            {
                _count500 -= 1;
                _count100 += 5;
                return true;
            }
            return false;
        }

     
        public void FillCoin100(int count)
        {
            _count100 += count;
        }
    }
}
