using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class test_camera_image : MonoBehaviour
{
    #region PRIVATE_MEMBERS
    private PIXEL_FORMAT mPixelFormat = PIXEL_FORMAT.GRAYSCALE;
    private bool mAccessCameraImage = true;
    private bool mFormatRegistered = false;
    #endregion // PRIVATE_MEMBERS


    // Start is called before the first frame update
    void Start()
    {
        // Register Vuforia life-cycle callbacks:
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);
    }

    private void OnVuforiaStarted()
    {
        // Vuforia has started, now register camera image format  
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError(
              "Failed to register pixel format " + mPixelFormat.ToString() +
              "\n the format may be unsupported by your device;" +
              "\n consider using a different pixel format.");

            mFormatRegistered = false;
        }
    }

    void OnTrackablesUpdated()
    {
        if (mFormatRegistered)
        {
            if (mAccessCameraImage)
            {
                Vuforia.Image image = CameraDevice.Instance.GetCameraImage(mPixelFormat);
                //if (image != null)
                //{
                //    Debug.Log(
                //        "\nImage Format: " + image.PixelFormat +
                //        "\nImage Size:   " + image.Width + "x" + image.Height +
                //        "\nBuffer Size:  " + image.BufferWidth + "x" + image.BufferHeight +
                //        "\nImage Stride: " + image.Stride + "\n"
                //    );
                //    byte[] pixels = image.Pixels;
                //    if (pixels != null && pixels.Length > 0)
                //    {
                //        Debug.Log(
                //            "\nImage pixels: " +
                //            pixels[0] + ", " +
                //            pixels[1] + ", " +
                //            pixels[2] + ", ...\n"
                //        );
                //    }
                //}

                byte[] pixels_mat = image.Pixels;
                //Debug.Log("Image Size: " + image.Width + "x" + image.Height);
                //Debug.Log("pixels_mat.length: " + pixels_mat.Length);
                //Texture2D text = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                Texture2D text = new Texture2D(1, 1);
                text.LoadRawTextureData(pixels_mat);

                //print("pixels_mat.Length: " + pixels_mat.Length);
                //print("text.width: " + text.width);
                //print("text.height: " + text.height);
                
                Sprite sprite = Sprite.Create(text, new Rect(0, 0, text.width, text.height), new Vector2(.5f, .5f));
        
                GameObject test_box = GameObject.Find("Test_button");

                if (test_box)
                {
                    SpriteRenderer spriteRenderer = test_box.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = sprite;
                  
                    for (int i = 0; i < 0; i++)
                    {
                        Debug.Log("pixels_mat[i]: " + pixels_mat[i]);
                        Debug.Log("text.GetPixel(i, 0): " + text.GetPixel(i, 0));
                    }
                }
                

            }
        }
    }


    void OnPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
            Debug.Log("App was resumed");
            RegisterFormat();
        }
    }
    /// 
    /// Register the camera pixel format
    /// 
    void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = false;
        }
    }
    /// 
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// 
    void UnregisterFormat()
    {
        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }



    // Update is called once per frame
    void Update()
    {

    }
}
