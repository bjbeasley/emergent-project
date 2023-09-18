using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PortraitProfile", menuName = "ScriptableObjects/PortraitProfile", order = 1)]
public class PortraitProfile : ScriptableObject
{

    //Skull shape
    [Header("Skull Shape")]

    public RandomParameter skullRoundness;
    public RandomParameter skullSmoothness;
    public RandomParameter skullWidth;
    public RandomParameter skullHeight;

    //Jawline
    [Header("Jawline")]

    public RandomParameter jawRoundness;
    public RandomParameter jawSmoothness;
    public RandomParameter jawWidth;
    public RandomParameter jawHeight;
    public RandomParameter jawOffset;
    public RandomParameter chinPointedness;

    //Neck
    [Header("Neck")]

    public RandomParameter neckWidth;

    //Ears
    [Header("Ears")]

    public RandomParameter earWidth;
    public RandomParameter earHeight;

    //Nose size
    [Header("Nose")]

    public RandomParameter noseWidth;
    public RandomParameter noseHeight;

    //Nose shape
    public RandomParameter noseProminence;
    public RandomParameter nostrilDefintion;

    //Eyebrows
    [Header("Eyebrows")]

    public RandomParameter eyebrowCurveDepth;
    public RandomParameter eyebrowCurveOffset;
    public RandomParameter eyebrowThickness;
    public RandomParameter eyebrowHeight;

    //Eyes
    [Header("Eyes")]

    public RandomParameter eyeWidth;
    public RandomParameter eyeHeight;
    public RandomParameter eyeSlant;
    public RandomParameter upperPeakSize;
    public RandomParameter upperPeakOffset;
    public RandomParameter lowerPeakSize;
    public RandomParameter lowerPeakOffset;
    public RandomParameter lashThickness;
    public RandomParameter lashWingThickness;

    [Header("Hair")]
    public float hasHairChance = 1;
    public RandomParameter hairlineHeight;


    [Header("Mouth")]
    public RandomParameter mouthWidth;
    public RandomParameter topLipSize;
    public RandomParameter bottomLipSize;

    public Portrait GetPortrait (bool infected)
    {
        Portrait portrait = new Portrait()
        {
            SkullRoundness = skullRoundness.GetValue(),
            SkullSmoothness = skullSmoothness.GetValue(),
            SkullSize = new Vector2(skullWidth.GetValue(), skullHeight.GetValue()),

            JawRoundness = jawRoundness.GetValue(),
            JawSmoothness = jawSmoothness.GetValue(),
            JawOffset = jawOffset.GetValue(),
            JawSize = new Vector2(jawWidth.GetValue(), jawHeight.GetValue()),
            ChinPoint = chinPointedness.GetValue(),

            NeckWidth = neckWidth.GetValue(),

            EarSize = new Vector2(earWidth.GetValue(), earHeight.GetValue()),

            NoseSize = new Vector2(noseWidth.GetValue(), noseHeight.GetValue()),

            NoseProminence = noseProminence.GetValue(),
            NostrilDefintion = nostrilDefintion.GetValue(),

            EyebrowCurveDepth = eyebrowCurveDepth.GetValue(),
            EyebrowCurveOffset = eyebrowCurveOffset.GetValue(),
            EyebrowHeight = eyebrowHeight.GetValue(),
            EyebrowThickness = eyebrowThickness.GetValue(),

            EyeSize = new Vector2(eyeWidth.GetValue(), eyeHeight.GetValue()),
            EyeSlant = eyeSlant.GetValue(),

            UpperLidOffset = upperPeakOffset.GetValue(),
            UpperLidPeak = upperPeakSize.GetValue(),

            LowerLidOffset = lowerPeakOffset.GetValue(),
            LowerLidPeak = lowerPeakSize.GetValue(),

            LashThickness = lashThickness.GetValue(),
            LashWingThickness = lashWingThickness.GetValue(),

            HasHair = Random.value < hasHairChance,
            HairLineHeight = hairlineHeight.GetValue(),

            MouthWidth = mouthWidth.GetValue(),
            TopLipHeight = topLipSize.GetValue(),
            BottomLipHeight = bottomLipSize.GetValue(),

            EyeColor = new Vector2(Random.value, Random.value),
            SkinColor = new Vector2(Random.value, Random.value),
            Infected = infected,
            
        };

        portrait.Generate();

        return portrait;
    }



}
