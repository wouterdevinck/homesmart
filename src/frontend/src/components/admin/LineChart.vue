<template>
  <Line :options="chartOptions" :data="chartData" />
</template>

<script>
import { Line } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, PointElement, LineElement, CategoryScale, TimeSeriesScale } from 'chart.js'
import 'chartjs-adapter-luxon';
ChartJS.register(Title, Tooltip, Legend, PointElement, LineElement, CategoryScale, TimeSeriesScale)
export default {
  components: { Line },
  props: ['data'],
  data() {
    return {
      chartData: {
        labels: this.data.points?.map(x => x.time),
        datasets: [ { 
          data: this.data.points?.map(x => x.value),
          backgroundColor: '#9EA1D4',
          borderColor: '#9EA1D4',
          tension: 0.4,
          unit: this.getUnitFormat(this.data.unit)
        } ]
      },
      chartOptions: {
        responsive: true,
        animation: false,
        scales: {
          x: {
            type: 'timeseries'
          }
        },
        plugins: {
          legend: {
            display: false
          },
          tooltip: {
            enabled: true,
            callbacks: {
              label: function(context) {
                return context.parsed.y + context.dataset.unit
              }
            }
          }
        }
      }
    }
  }
}
</script>