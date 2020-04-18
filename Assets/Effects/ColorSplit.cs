using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ColorSplitRenderer), PostProcessEvent.AfterStack, "Custom/ColorSplit")]
public sealed class ColorSplit : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Color split effect distance.")]
    public FloatParameter amount = new FloatParameter { value = 0.0f };

    public Vector2Parameter redOffset = new Vector2Parameter { value = Vector2.zero };
    public Vector2Parameter greenOffset = new Vector2Parameter { value = Vector2.zero };
    public Vector2Parameter blueOffset = new Vector2Parameter { value = Vector2.zero };
}

public sealed class ColorSplitRenderer : PostProcessEffectRenderer<ColorSplit>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/ColorSplit"));
        sheet.properties.SetFloat("_Amount", settings.amount);
        sheet.properties.SetVector("_RedOffset", settings.redOffset);
        sheet.properties.SetVector("_GreenOffset", settings.greenOffset);
        sheet.properties.SetVector("_BlueOffset", settings.blueOffset);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}