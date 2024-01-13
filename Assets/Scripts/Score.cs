using UnityEngine;

public class Score : MonoBehaviour
{
    private int score = 0;
    public static Score Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Phương thức để tăng điểm số
    public void IncreaseScore(int amount)
    {
        score += amount;
        Debug.Log("Score increased by " + amount + ". Total score: " + score);
        // Gọi các hàm hoặc xử lý khác sau khi điểm số tăng lên
    }

    // Phương thức để lấy điểm số hiện tại
    public int GetScore()
    {
        return score;
    }

    // Phương thức để đặt lại điểm số về 0
    public void ResetScore()
    {
        score = 0;
        Debug.Log("Score reset to 0");
        // Gọi các hàm hoặc xử lý khác sau khi điểm số được đặt lại
    }
}
