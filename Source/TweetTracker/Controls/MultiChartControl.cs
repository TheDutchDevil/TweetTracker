using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Windows.Controls.DataVisualization.Charting;
using TweetTracker.Util.StyleProviders;
using TweetTracker.ViewModels.DataModel;

namespace TweetTracker.Controls
{
    public class MultipleChart : System.Windows.Controls.DataVisualization.Charting.Chart
    {

        #region SeriesSource (DependencyProperty)

        public IEnumerable SeriesSource
        {
            get { return (IEnumerable)GetValue(SeriesSourceProperty); }
            set { SetValue(SeriesSourceProperty, value); }
        }
        public static readonly DependencyProperty SeriesSourceProperty = DependencyProperty.Register("SeriesSource", typeof(IEnumerable), typeof(MultipleChart), new PropertyMetadata(default(IEnumerable), MultipleChart.OnSeriesSourceChanged));

        private static void OnSeriesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IEnumerable oldValue = (IEnumerable)e.OldValue;
            IEnumerable newValue = (IEnumerable)e.NewValue;
            MultipleChart source = (MultipleChart)d;
            source.OnSeriesSourceChanged(oldValue, newValue);
        }

        protected virtual void OnSeriesSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            this.Series.Clear();

            if (newValue != null)
            {
                foreach (object item in newValue)
                {
                    DataTemplate dataTemplate = null;

                    // get data template
                    if (this.SeriesTemplateSelector != null)
                    {
                        dataTemplate = this.SeriesTemplateSelector.SelectTemplate(item, this);
                    }
                    if (dataTemplate == null && this.SeriesTemplate != null)
                    {
                        dataTemplate = this.SeriesTemplate;
                    }

                    if(dataTemplate == null)
                    {
                        throw new InvalidOperationException("Invalid data lead to an invalid state for this multi chart control");
                    }

                    if (dataTemplate.LoadContent() is LineSeries)
                    {
                        var lineSeries = dataTemplate.LoadContent() as LineSeries;

                        lineSeries.DataContext = item;
                        lineSeries.DataPointStyle = DataPointStyleProvider.StyleForSubjectMapper(item as CaptureSubjectMapper);

                        this.Series.Add(lineSeries);
                    }
                    else
                    {
                        Series series = dataTemplate.LoadContent() as Series;

                        if (series != null)
                        {
                            // set data context
                            series.DataContext = item;

                            this.Series.Add(series);
                        }
                    }
                }
            }

            // TODO Listen for INotifyCollectionChanged with a weak event pattern
        }

        #endregion

        #region SeriesTemplate (DependencyProperty)

        public DataTemplate SeriesTemplate
        {
            get { return (DataTemplate)GetValue(SeriesTemplateProperty); }
            set { SetValue(SeriesTemplateProperty, value); }
        }
        public static readonly DependencyProperty SeriesTemplateProperty = DependencyProperty.Register("SeriesTemplate", typeof(DataTemplate), typeof(MultipleChart), new PropertyMetadata(default(DataTemplate)));

        #endregion

        #region SeriesTemplateSelector (DependencyProperty)

        public DataTemplateSelector SeriesTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SeriesTemplateSelectorProperty); }
            set { SetValue(SeriesTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty SeriesTemplateSelectorProperty = DependencyProperty.Register("SeriesTemplateSelector", typeof(DataTemplateSelector), typeof(MultipleChart), new PropertyMetadata(default(DataTemplateSelector)));

        #endregion

    }
}
