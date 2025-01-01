<template>
  <Line :options="chartOptions" :data="chartData" />
</template>

<script>
import { Line } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, PointElement, LineElement, CategoryScale, LinearScale } from 'chart.js'
ChartJS.register(Title, Tooltip, Legend, PointElement, LineElement, CategoryScale, LinearScale)
export default {
  components: { Line },
  props: ['data'],
  data() {
    return {
      chartData: {
        labels: this.data.points?.map(x => x.time), // TODO map to labels depending on window (or do server side?)
        datasets: [ { 
          data: this.data.points?.map(x => x.value), // TODO Add zeros for missing data points or do server side?
          backgroundColor: '#9EA1D4',
          borderColor: '#9EA1D4',
          tension: 0.4,
          unit: this.getUnitFormat(this.data.unit)
        } ]
      },
      chartOptions: {
        responsive: true,
        animation: false,
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