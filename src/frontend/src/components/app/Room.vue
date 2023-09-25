<template>
  <div>
    <div class="card mb-3">
      <div class="card-body"> 
        <div class="roomname"> 
          <svg class="bi me-2" width="50" height="50">
            <use v-bind:xlink:href="'#' + getRoomIcon(room)"/>
          </svg>
          <span>{{ room.name }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: ['room']
}
</script>

<style scoped>
@import '../../assets/card.css';
.roomname {
  font-weight: bold;
}
.roomname > * {
  vertical-align: middle;
}
.roomname > span {
  margin-left: 5px;
}
/*.card {
  height: 80px;
}*/
</style>