<template>
  <div>
      <Line
        id="my-chart-id"
        :options="chartOptions"
        :data="chartData"
      />
    <!--div v-for="point in data">
      <span v-html="point.value"></span>
    </div-->
  </div>
</template>

<script>
// https://www.chartjs.org/docs/latest/samples/line/styling.html
// https://vue-chartjs.org/guide/

import api from '../api'

import { Line } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, PointElement, LineElement, CategoryScale, LinearScale, Filler } from 'chart.js'

ChartJS.register(Title, Tooltip, Legend, PointElement, LineElement, CategoryScale, LinearScale, Filler)

export default {

components: { Line },
  data() {
    return {
      chartData: {
        labels: [ 'January', 'February', 'March' ],
        datasets: [ { 
          data: [40, 20, 12],
          //cubicInterpolationMode: 'monotone',
          tension: 0.4,
          borderColor: 'rgb(54, 162, 235)',
          //backgroundColor: 'rgb(255, 99, 132)'
          fill: 'origin',
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
            enabled: false
          }
        }
      },
      data: null
    }
  },

  // props: ['device'],
  //computed: {
  //  data: api.getData()
  //}  
  //created () {
    //this.$store.dispatch('getData') // TODO params
  //}
  //data () {
  //  return {
  //    data: null
  //  }
  //},
  mounted () {
    api.getData(data => {
      this.data = data
    }, 'sensor-shed', 'temperature')
  }
}
</script>

<style scoped>

</style>