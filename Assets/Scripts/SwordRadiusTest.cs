using NUnit.Framework.Internal;
using UnityEngine;
using UnityEditor;


public class SwordRadiusTest
{
    // Insert Input for the targeted Mouse X and Y
    // Test Function
    //{
    //[SerializeField] private WeaponSwing testPosition;

    //WeaponSwing testPosition = new WeaponSwing();
    //[MenuItem("Tools/SwordOutput")]
    public static void RunScript()
    {
        TestAndExplain(50, -50, 0, 0, "UpRight", "Sword Positioned Upright");
        TestAndExplain(20, 20, 0, 0, "UpLeft", "Sword Positioned UpLeft");
        TestAndExplain(-10, -10, 0, 0, "DownRight", "Sword Positioned DownRight");
        TestAndExplain(-30, 20, 0, 0, "DownLeft", "Sword Positioned DownLeft");

        TestAndExplain(50, -50, 10, 10, "UpRight", "Sword Positioned Upright (Player Moved)");
        TestAndExplain(20, 20, 1, 1, "UpLeft", "Sword Positioned UpLeft (Player Moved)");
        TestAndExplain(10, 10, 90, 90, "DownRight", "Sword Positioned DownRight (Player Moved)");
        TestAndExplain(-30, -20, -10, -50, "DownLeft", "Sword Positioned DownLeft (Player Moved)");

    }
    private static (bool,string) getResult(float testMouseX, float testMouseY, float testPlayerX, float testPlayerY, string ExpectedResult)
        {   
            WeaponSwing testPosition = Object.FindAnyObjectByType<WeaponSwing>();
            var (result1, result2) = testPosition.CalculatePosition(testMouseX, testMouseY, testPlayerX, testPlayerY);
            bool overX = result1 - testPlayerX > 0;
            bool overY = result2 - testPlayerY > 0;
            string finalResult = "None";

            if (overX && overY)
            {
                // Point UP Right
                finalResult = "UpRight";
            }
            else if (!overX && overY)
            {
                //Point UP Left
                finalResult = "UpLeft";
            }
            else if (overX && !overY)
            {
                // Point DOWN Right
                finalResult = "DownRight";
            }
            else
            {
                //Point DOWN Left
                finalResult = "DownLeft";
            }

            return (finalResult == ExpectedResult,finalResult);
        }

        private static void TestAndExplain(float testMouseX, float testMouseY, float testPlayerX, float testPlayerY, string ExpectedResult, string message)
        {
            UnityEngine.Debug.Log("Testing - - - " + message);
            var (testSucess,finalResult) = getResult(testMouseX, testMouseY, testPlayerX, testPlayerY, ExpectedResult);
            if (testSucess)
            {
                UnityEngine.Debug.Log("Test Success!");
            }
            else
            {
                UnityEngine.Debug.Log("Incorrect Result - - - Expected: " + ExpectedResult + " Got: " + finalResult);
            }
        }


    public static void ExecuteScript()
    {
        TestAndExplain(50, -50, 0, 0, "UpRight", "Sword Positioned Upright");
        TestAndExplain(20, 20, 0, 0, "UpLeft", "Sword Positioned UpLeft");
        TestAndExplain(-10, -10, 0, 0, "DownRight", "Sword Positioned DownRight");
        TestAndExplain(-30, 20, 0, 0, "DownLeft", "Sword Positioned DownLeft");

        TestAndExplain(50, -50, 10, 10, "UpRight", "Sword Positioned Upright (Player Moved)");
        TestAndExplain(20, 20, 1, 1, "UpLeft", "Sword Positioned UpLeft (Player Moved)");
        TestAndExplain(10, 10, 90, 90, "DownRight", "Sword Positioned DownRight (Player Moved)");
        TestAndExplain(-30, -20, -10, -50, "DownLeft", "Sword Positioned DownLeft (Player Moved)");

    }
}
    

