using UnityEngine;
using TMPro;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEditor.Searcher.SearcherWindow;
using System.Drawing;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float countdownTime = 180f; // 3 minutes in seconds

    [Header("UI Reference")]
    public TextMeshProUGUI timerText; // Drag your UI text here in Inspector

    private float currentTime;
    private bool timerRunning = true;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = countdownTime; // Start at 3 minutes
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            // Countdown
            currentTime -= Time.deltaTime;

            // Check if timer reached zero
            if (currentTime <= 0)
            {
                currentTime = 0;
                timerRunning = false;
                TimeUp();
            }

            // Update the UI
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void TimeUp()
    {
        Debug.Log("Time's up! Game Over!");

        // Add your game over logic here
    }
}
//```

//5.Press * *Ctrl + S * *to save
//6. Go back to Unity (click the Unity window or Alt+Tab)
//7. Wait for Unity to finish compiling (you'll see a progress bar at the bottom)

//---

//## **STEP 3: Create the Timer GameObject**

//1. In the **Hierarchy** (left side), right-click in empty space
//2. Choose **Create Empty**
//3. Name it **"GameTimer"** (click on it and press F2 to rename)
//4. With "GameTimer" selected, look at the **Inspector** (right side)
//5. Click **Add Component** at the bottom
//6. Type "GameTimer" and click on your GameTimer script
//7. You should now see **"Game Timer (Script)"** in the Inspector

//---

//## **STEP 4: Create the UI Text Display**

//1. In the **Hierarchy**, right-click in empty space
//2. Go to **UI ? Text - TextMeshPro**
//3. Unity might pop up a window saying "Import TMP Essentials" ? Click **"Import TMP Essentials"**, then **"Close"**
//4. You should now see in Hierarchy:
//   -**Canvas * *
//   -**Text(TMP) * *(under Canvas)
//   - **EventSystem**

//5. **Select** the **Text (TMP)** object
//6. **Rename it** to "TimerText" (press F2)

//---

//## **STEP 5: Position and Style the Timer Text**

//1. With **TimerText** selected in Hierarchy
//2. In the **Inspector** (right side), find **Text Mesh Pro - Text (UI)**
//3. Change these settings:
//   -**Text Input Box**: Type `03:00`
//   -**Font Size * *: `48`
//   -**Alignment * *: Click the **center** button (middle of the 9 squares)
//   - **Color**: Change to white or whatever you want

//4. **Position it at the top of screen:**
//   -At the top of the Inspector, find **Rect Transform**
//   - Change **Pos Y** to around `450` (moves it to top)
//   - You should see it move to the top center in the **Game view**

//---

//## **STEP 6: Connect the Script to the UI**

//1. Select **GameTimer** object in the Hierarchy
//2. Look at the **Inspector**
//3. Find the **Game Timer (Script)** section
//4. You should see:
//```
//   Countdown Time: 180
//   Timer Text: None(Text Mesh Pro UGUI)