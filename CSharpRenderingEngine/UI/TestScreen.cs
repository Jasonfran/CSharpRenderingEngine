using System;
using NotImplementedException = System.NotImplementedException;

namespace Engine.UI
{
    public class TestScreen : UIScreen
    {
        private UIButton testButton;

        public TestScreen()
        {
            testButton = AddComponent(new UIButton(100, 100, 100, 100));
            testButton.OnClick += TestButtonOnOnClick;
        }

        private void TestButtonOnOnClick(object sender, UIButtonEventArgs e)
        {
            Console.WriteLine("Test button clicked. It works!");
        }
    }
}