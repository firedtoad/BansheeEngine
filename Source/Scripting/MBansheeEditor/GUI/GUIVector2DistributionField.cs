﻿using System;
using BansheeEngine;

namespace BansheeEditor
{
    partial class GUIVector2DistributionField
    {
        /// <summary>
        /// Triggered when the distribution in the field changes.
        /// </summary>
        public event Action OnChanged;

        /// <summary>
        /// Triggered whenever user confirms input in one of the floating point fields.
        /// </summary>
        public event Action OnConfirmed;

        partial void OnClicked(int component)
        {
            Vector2Distribution distribution = Value;

            if (distribution.DistributionType == PropertyDistributionType.Curve)
            {
                AnimationCurve[] curves = AnimationUtility.SplitCurve2D(distribution.GetMinCurve());
                if (component < curves.Length)
                {
                    CurveEditorWindow.Show(curves[component], (success, curve) =>
                    {
                        if (!success)
                            return;

                        curves[component] = curve;

                        Vector2Curve compoundCurve = AnimationUtility.CombineCurve2D(curves);
                        Value = new Vector2Distribution(compoundCurve);
                        OnChanged?.Invoke();
                    });
                }
            }
            else if (distribution.DistributionType == PropertyDistributionType.RandomCurveRange)
            {
                AnimationCurve[] minCurves = AnimationUtility.SplitCurve2D(distribution.GetMinCurve());
                AnimationCurve[] maxCurves = AnimationUtility.SplitCurve2D(distribution.GetMaxCurve());

                if (component < minCurves.Length && component < maxCurves.Length)
                {
                    CurveEditorWindow.Show(minCurves[component], maxCurves[component],
                        (success, minCurve, maxCurve) =>
                        {
                            if (!success)
                                return;

                            minCurves[component] = minCurve;
                            maxCurves[component] = maxCurve;

                            Vector2Curve minCompoundCurves = AnimationUtility.CombineCurve2D(minCurves);
                            Vector2Curve maxCompoundCurves = AnimationUtility.CombineCurve2D(maxCurves);

                            Value = new Vector2Distribution(minCompoundCurves, maxCompoundCurves);
                            OnChanged?.Invoke();
                        });
                }
            }
        }

        partial void OnConstantModified()
        {
            OnChanged?.Invoke();
        }

        partial void OnConstantConfirmed()
        {
            OnConfirmed?.Invoke();
        }
    }
}