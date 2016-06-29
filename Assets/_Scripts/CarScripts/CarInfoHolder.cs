using UnityEngine;
using System.Collections;

public class CarInfoHolder : MonoBehaviour
{
    public tk2dSprite HealthBar;
    public CarScript Car;

    public Color FullHealthColor, EmptyHealthColor;

    float _initScale;

    void Awake()
    {
        _initScale = HealthBar.transform.localScale.x;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);

        float curHealth = Car.CurHealth;

        float coef = 1f;
        
        if (!Car.IsKing)
            coef = curHealth / Car.InitHealth;
        else
            coef = curHealth / (Car.InitHealth * 2);

        HealthBar.transform.localScale = new Vector3(coef * _initScale, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);

        Color newColor = Color.Lerp(EmptyHealthColor, FullHealthColor, coef);

        float h, s, v;
        Color.RGBToHSV(newColor, out h, out s, out v);
        Color colorWithFullV = Color.HSVToRGB(h, s, 1f);

        HealthBar.color = colorWithFullV;
    }
}
