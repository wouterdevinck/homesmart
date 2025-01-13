<template>
  <Bar :options="chartOptions" :data="chartData" />
</template>

<script>
import { Bar } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale } from 'chart.js'
ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale)
export default {
  components: { Bar },
  props: ['data'],
  data() {
    return {
      chartData: {
        labels: this.data.points?.map(x => x.label),
        datasets: [ { 
          data: this.data.points?.map(x => x.value),
          backgroundColor: '#9EA1D4',
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