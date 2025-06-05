using System;
using System.Numerics;

namespace OccultCrescentHelper;

public static unsafe class CameraHelper
{
    public static bool WorldLineToScreen(
        Vector3 startWorld,
        Vector3 endWorld,
        Matrix4x4 viewMatrix,
        Matrix4x4 projectionMatrix,
        float nearPlane,
        uint screenWidth,
        uint screenHeight,
        out Vector2 screenStart,
        out Vector2 screenEnd)
    {
        screenStart = default;
        screenEnd = default;

        Vector4 start4 = new Vector4(startWorld, 1f);
        Vector4 end4 = new Vector4(endWorld, 1f);

        Vector4 startView = Vector4.Transform(start4, viewMatrix);
        Vector4 endView = Vector4.Transform(end4, viewMatrix);

        if (!ClipLineToNearPlane(ref startView, ref endView, nearPlane))
            return false;

        Vector4 startClip = Vector4.Transform(startView, projectionMatrix);
        Vector4 endClip = Vector4.Transform(endView, projectionMatrix);

        if (startClip.W == 0 || endClip.W == 0)
            return false;

        Vector3 startNDC = new Vector3(startClip.X, startClip.Y, startClip.Z) / startClip.W;
        Vector3 endNDC = new Vector3(endClip.X, endClip.Y, endClip.Z) / endClip.W;

        startNDC.X = Math.Clamp(startNDC.X, -1f, 1f);
        startNDC.Y = Math.Clamp(startNDC.Y, -1f, 1f);
        endNDC.X = Math.Clamp(endNDC.X, -1f, 1f);
        endNDC.Y = Math.Clamp(endNDC.Y, -1f, 1f);

        // Convert NDC to screen coordinates
        screenStart = new Vector2(
            (startNDC.X + 1f) * 0.5f * screenWidth,
            (1f - (startNDC.Y + 1f) * 0.5f) * screenHeight);

        screenEnd = new Vector2(
            (endNDC.X + 1f) * 0.5f * screenWidth,
            (1f - (endNDC.Y + 1f) * 0.5f) * screenHeight);

        return true;
    }

    private static bool ClipLineToNearPlane(ref Vector4 startView, ref Vector4 endView, float nearPlane)
    {
        float zNear = -nearPlane;

        bool startBehind = startView.Z > zNear;
        bool endBehind = endView.Z > zNear;

        if (startBehind && endBehind)
            return false;

        if (startBehind)
        {
            float t = (zNear - startView.Z) / (endView.Z - startView.Z);
            startView = Vector4.Lerp(startView, endView, t);
        }
        else if (endBehind)
        {
            float t = (zNear - startView.Z) / (endView.Z - startView.Z);
            endView = Vector4.Lerp(startView, endView, t);
        }

        return true;
    }

    public static Vector3 WorldToScreen(Vector3 pointWorld, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, uint windowWidth, uint windowHeight)
    {
        Vector4 pointWorld4 = new Vector4(pointWorld, 1f);
        Vector4 pointView = Vector4.Transform(pointWorld4, viewMatrix);
        Vector4 pointClip = Vector4.Transform(pointView, projectionMatrix);

        if (pointClip.W <= 0)
            return new Vector3(-1, -1, -1); // behind camera sentinel

        Vector3 pointNDC = new Vector3(pointClip.X, pointClip.Y, pointClip.Z) / pointClip.W;

        Vector3 clampedNDC = new Vector3(
            Math.Clamp(pointNDC.X, -1, 1),
            Math.Clamp(pointNDC.Y, -1, 1),
            pointNDC.Z);

        float screenX = (clampedNDC.X + 1f) * 0.5f * windowWidth;
        float screenY = (1f - (clampedNDC.Y + 1f) * 0.5f) * windowHeight;

        return new Vector3(screenX, screenY, clampedNDC.Z);
    }
}
