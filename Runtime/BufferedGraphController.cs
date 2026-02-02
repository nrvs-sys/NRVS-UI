using System.Collections.Generic;
using ChartAndGraph;
using UnityEngine;

public class BufferedGraphController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] 
    GraphChartBase graph;

    [Header("Rolling-buffer Settings")]
    [SerializeField, Min(1)]
    int maxSamples = 200;

    [SerializeField, Tooltip("If true, the graph's Horizontal Size & Offset will be recalculated to match the current amount of samples.")]
    bool fillGraph = true;

    [Header("Auto-Normalization Settings")]
    [SerializeField, Tooltip("When true, each time you add a sample the Y-axis will expand to fit your data.")]
    bool autoNormalizeY = true;
    [SerializeField, Range(0f, 1f), Tooltip("How much extra headroom above the highest point (10% = 0.1).")]
    float yPadding = 0.1f;

    // One FIFO buffer per category
    readonly Dictionary<string, Queue<DoubleVector2>> buffers = new Dictionary<string, Queue<DoubleVector2>>();


    /// <summary
    /// >Add a new data point to a category (category must already exist in the chart).
    /// </summary>
    public void AddSample(string category, double x, double y)
    {
        if (graph == null) return;

        // Maintain the rolling buffer
        if (!buffers.TryGetValue(category, out var q))
            buffers[category] = q = new Queue<DoubleVector2>(maxSamples);

        q.Enqueue(new DoubleVector2(x, y));
        while (q.Count > maxSamples)
            q.Dequeue();

        // Rebuild the category in one batch
        var ds = graph.DataSource;
        ds.StartBatch();
        ds.ClearCategory(category);
        foreach (var pt in q)
        {
            ds.AddPointToCategory(category, pt.x, pt.y);
        }
        ds.EndBatch();

        // Adjust the horizontal view size and offse
        if (fillGraph)
        {
            var origin = q.Count > 0 ? q.Peek().x : 0;
            ds.HorizontalViewSize = x - origin;
            ds.HorizontalViewOrigin = origin;
        }

        // Optionally auto-scale Y
        if (autoNormalizeY)
        {
            // Compute the global maximum across all categories
            double maxY = 0;
            foreach (var buf in buffers.Values)
            {
                foreach (var pt in buf)
                    if (pt.y > maxY) maxY = pt.y;
            }
            // add a little padding on top
            ds.VerticalViewSize = maxY * (1f + yPadding);
        }
    }

    /// <summary>
    /// Change the rolling-buffer length at runtime.
    /// </summary>
    public void SetMaxSamples(int newMax)
    {
        maxSamples = Mathf.Max(1, newMax);
        // trim all buffers
        foreach (var q in buffers.Values)
            while (q.Count > maxSamples)
                q.Dequeue();
    }

    /// <summary>
    /// Enable or disable auto-normalization at runtime.
    /// </summary>
    public void SetAutoNormalizeY(bool on) => autoNormalizeY = on;

    /// <summary>
    /// Clear a whole category, buffer included.
    /// </summary>
    public void ClearCategory(string category)
    {
        buffers.Remove(category);
        graph?.DataSource.ClearCategory(category);
    }
}
