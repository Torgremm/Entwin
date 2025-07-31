window.renderPlot = (x, y, label) => {
    const trace = {
        x: x,
        y: y,
        type: 'scatter',
        mode: 'lines',
        name: label
    };

    const layout = {
        title: label,
        xaxis: { title: 'Time (s)' },
        yaxis: { title: 'Value', autorange: true }
    };

    Plotly.newPlot('plotly-chart', [trace], layout);
};
