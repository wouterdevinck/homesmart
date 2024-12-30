<template>
  <Bar
    id="my-chart-id"
    :options="chartOptions"
    :data="chartData"
  />
</template>

<script>
import { Bar } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale } from 'chart.js'
ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale)
export default {
  name: 'BarChart',
  components: { Bar },
  props: ['data'],
  data() {
    return {
      chartData: {
        labels: this.data.map(x => x.time), // TODO map to labels depending on window (or do server side?)
        datasets: [ { 
          data: this.data.map(x => x.value), // TODO Add zeros for missing data points or do server side?
          backgroundColor: '#9EA1D4'
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
                return context.parsed.y // + ' unit' // TODO Get correct unit from server? From metadata from configuration?
              }
            }
          }
        }
      }
    }
  }
}
</script>