using System;
using Xunit;
using System.Collections.Generic;
using XingeApp;
namespace SDK.Test
{
    public class UnitTest1
    {
         [Fact]
        public void TestPushTokenOfiOS()
        {
            string result = null;
            Random random = new Random();
            int messageIndex = random.Next(9999);
            result = XingeApp.XingeApp.PushTokeniOs("5412567459229", 2200262432, "24f595178abda55aa74dc2ce4033d600", "CSharpSDK " + messageIndex,"1043b93f56334bf010d40ccefa1244d8557a45161d55ff3ecbb7e1b13e192f65", XingeApp.XingeApp.PushEnvironmentofiOs.Develop);
            Console.WriteLine(result);
        }
        [Fact]
        public void TestPushTokenOfAndroid()
        {
            string result = null;
            Random random = new Random();
            int messageIndex = random.Next(9999);
            result = XingeApp.XingeApp.PushTokenAndroid("d617a675b62d0",2100271539, "9e05364c6d56da943783e61da091e8e5","Android","CSharpSDK " + messageIndex, "9579bab6e7d7533f70e8e0be2e45d01fe1b88b2f");
            Console.WriteLine(result);
        }
        [Fact]
        public void TestPushAccountOfiOS()
        {
            string result = null;
            Random random = new Random();
            int messageIndex = random.Next(9999);
            result = XingeApp.XingeApp.PushAccountiOs("5412567459229", 2200262432, "24f595178abda55aa74dc2ce4033d600", "CSharpSDK " + messageIndex,"your test account", XingeApp.XingeApp.PushEnvironmentofiOs.Develop);
            Console.WriteLine(result);
        }
        [Fact]
        public void TestPushAccountOfAndroid()
        {
            string result = null;
            Random random = new Random();
            int messageIndex = random.Next(9999);
            result = XingeApp.XingeApp.PushAccountAndroid("d617a675b62d0",2100271539, "9e05364c6d56da943783e61da091e8e5","Android","CSharpSDK " + messageIndex, "your test account");
            Console.WriteLine(result);
        }
        [Fact]
        public void TestPushTagOfiOS()
        {
            string result = null;
            Random random = new Random();
            int messageIndex = random.Next(9999);
            result = XingeApp.XingeApp.PushTagiOs("5412567459229", 2200262432, "24f595178abda55aa74dc2ce4033d600", "CSharpSDK " + messageIndex,"your_test_tag", XingeApp.XingeApp.PushEnvironmentofiOs.Develop);
            Console.WriteLine(result);
        }
        
        [Fact]
        public void TestPushTagOfAndroid()
        {
            string result = null;
            Random random = new Random();
            int messageIndex = random.Next(9999);
            result = XingeApp.XingeApp.PushTagAndroid("d617a675b62d0",2100271539, "9e05364c6d56da943783e61da091e8e5","Android","CSharpSDK " + messageIndex, "your_test_tag");
            Console.WriteLine(result);
        }
        
        [Fact]
        public void TestPushAllOfiOS()
        {
            string result = null;
            Random random = new Random();
            int messageIndex = random.Next(9999);
            result = XingeApp.XingeApp.PushAlliOs("5412567459229", 2200262432, "24f595178abda55aa74dc2ce4033d600", "CSharpSDK " + messageIndex, XingeApp.XingeApp.PushEnvironmentofiOs.Develop);
            Console.WriteLine(result);
        }
        [Fact]
        public void TestPushAllOfAndroid()
        {
            string result = null;
            Random random = new Random();
            int messageIndex = random.Next(9999);
            result = XingeApp.XingeApp.PushAllAndroid("d617a675b62d0",2100271539, "9e05364c6d56da943783e61da091e8e5","Android","CSharpSDK"+ messageIndex);
            Console.WriteLine(result);
        }

    }
}
