#ifndef EDGE_EVENT_QUEUE
#define EDGE_EVENT_QUEUE
#include <pico/util/queue.h>

struct EdgeEvent
{
    uint64_t timestamp_us;
    uint32_t rise_pins;
    uint32_t fall_pins;
};

extern queue_t edge_event_queue;

#endif // EDGE_EVENT_QUEUE
