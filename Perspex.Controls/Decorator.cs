﻿// -----------------------------------------------------------------------
// <copyright file="Decorator.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Perspex.Collections;
    using Perspex.Layout;

    public class Decorator : Control, IVisual, ILogical
    {
        public static readonly PerspexProperty<Control> ChildProperty =
            PerspexProperty.Register<Decorator, Control>("Child");

        public static readonly PerspexProperty<Thickness> PaddingProperty =
            PerspexProperty.Register<Decorator, Thickness>("Padding");

        private PerspexSingleItemList<ILogical> logicalChild = new PerspexSingleItemList<ILogical>();

        public Decorator()
        {
            this.GetObservableWithHistory(ChildProperty).Subscribe(x =>
            {
                if (x.Item1 != null)
                {
                    this.RemoveVisualChild(x.Item1);
                    x.Item1.Parent = null;
                }

                if (x.Item2 != null)
                {
                    this.AddVisualChild(x.Item2);
                    x.Item2.Parent = this;
                }

                this.logicalChild.SingleItem = x.Item2;
            });
        }

        public Control Child
        {
            get { return this.GetValue(ChildProperty); }
            set { this.SetValue(ChildProperty, value); }
        }

        public Thickness Padding
        {
            get { return this.GetValue(PaddingProperty); }
            set { this.SetValue(PaddingProperty, value); }
        }

        IPerspexReadOnlyList<ILogical> ILogical.LogicalChildren
        {
            get { return this.logicalChild; }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Control content = this.Child;

            if (content != null)
            {
                content.Arrange(new Rect(finalSize).Deflate(this.Padding));
            }

            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var content = this.Child;
            var padding = this.Padding;

            if (content != null)
            {
                content.Measure(availableSize.Deflate(padding));
                return content.DesiredSize.Inflate(padding);
            }
            else
            {
                return new Size(padding.Left + padding.Right, padding.Bottom + padding.Top);
            }
        }
    }
}
