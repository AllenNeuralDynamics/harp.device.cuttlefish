#ifndef PWM_TASK_H
#define PWM_TASK_H
#include <stdint.h>
#include <pico/stdlib.h>
#include <hardware/gpio.h>
#ifdef DEBUG
    #include <cstdio> // for printf
#endif

/**
 * \brief container for bookkeeping update times/states of a PWM task.
 */
class PWMTask
{
public:
    // FIXME: pin should be a pin mask since this PWMTask could apply to multiple pins.
    PWMTask(uint32_t t_delay_us, uint32_t t_on_us, uint32_t t_period_us,
            uint32_t pin_mask, uint32_t count = 0, bool invert = false);

    ~PWMTask();

/**
 * \brief comparison operator for scheduling.
 */
    friend bool operator<(const PWMTask& lhs, const PWMTask& rhs)
    {return int32_t(rhs.next_update_time_us_ - lhs.next_update_time_us_) > 0;}

    enum update_state_t: uint8_t
    {
        LOW = 0,
        HIGH = 1
    };

/**
 * \brief should be called in a loop.
 * \param[force] if true, the pwm state change will forcibly iterate, which is
 *  useful if time/actions are managed by an external scheduler.
 * \param[skip_output_action] if true, this class will not manipulate the GPIO
 *  pins in the pin mask
 */
    void update(bool force = false, bool skip_output_action = false);

/**
 * \brief read-only public wrapper for the next absolute time that this
 *  instance must update.
 */
    const inline uint32_t next_update_time_us()
    {return next_update_time_us_;}

    // TODO: should(?) have access to Harp time in the future.
    inline void start(bool skip_output_action = false)
    {start_at_time(timer_hw->timerawl, skip_output_action);}

/**
 * \brief start the PWM state machine but override its start time to
 *  schedule it in the future.
 */
    inline void start_at_time(uint32_t start_time_us, bool skip_output_action = false)
    {
        start_time_us_ = start_time_us;
        reset(skip_output_action);
    }

/**
 * \brief true if an update is due/overdue.
 */
    inline bool time_to_update() // FIXME: should be harp time.
    {return int32_t(timer_hw->timerawl - next_update_time_us_) >= 0;}

/**
 * \brief true if update() must be called again in the future.
 */
    inline bool requires_future_update()
    {
        if (count_ == 0 || cycles_ != count_)
            return true;
        return false;
    }

private:
    friend class PWMScheduler;

    uint32_t delay_us_; /// pulse train delay (phase offset) in microseconds.
    uint32_t on_time_us_; /// pulse train duty cycle in microseconds
    uint32_t period_us_; /// pulse train period in microseconds

    uint32_t pin_mask_; /// active channels.
    update_state_t state_; /// current state of pulse waveform.
    uint32_t count_; /// How many pulses to issue.
                     ///  0: pulse forever. >0: execute N times.
    uint32_t cycles_; /// how many times we have pulsed.
    uint32_t start_time_us_; /// What (32-bit) time the pulse started.

/**
 * \brief absolute time that the state machine needs to update.
 */
    uint32_t next_update_time_us_;

    inline void reset(bool skip_output_action = false)
    {
        cycles_ = 0;
        gpio_put_masked(pin_mask_, 0);
        state_ = (delay_us_ == 0)? HIGH: LOW;
        next_update_time_us_ = start_time_us_ + delay_us_;
        if (state_ == HIGH)
            next_update_time_us_ += on_time_us_;
        if (skip_output_action)
            return;
        // Apply initial pwm state to gpio pins.
        uint32_t pin_state = (state_ == HIGH)? pin_mask_: 0;
        gpio_put_masked(pin_mask_, pin_state);
    }
};
#endif // PWM_TASK_H
