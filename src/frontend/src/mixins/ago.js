export default {
  methods: {
    getAgo: function(time) {
      return timeAgo(Date.parse(time))
    }
  }
}


// https://stackoverflow.com/questions/3177836/how-to-format-time-since-xxx-e-g-4-minutes-ago-similar-to-stack-exchange-site

const epochs = [
  ['year', 31536000],
  ['month', 2592000],
  ['day', 86400],
  ['hour', 3600],
  ['minute', 60],
  ['second', 1]
];

const getDuration = (timeAgoInSeconds) => {
  for (let [name, seconds] of epochs) {
    const interval = Math.floor(timeAgoInSeconds / seconds)
    if (interval >= 1) {
      return {
        interval: interval,
        epoch: name
      }
    }
  }
}

const timeAgo = (date) => {
  const timeAgoInSeconds = Math.floor((new Date() - new Date(date)) / 1000)
  const {interval, epoch} = getDuration(timeAgoInSeconds)
  const suffix = interval === 1 ? '' : 's'
  return `${interval} ${epoch}${suffix} ago`
}