cmake_minimum_required(VERSION 3.13)
find_package(Git REQUIRED)
execute_process(COMMAND "${GIT_EXECUTABLE}" rev-parse --short HEAD OUTPUT_VARIABLE COMMIT_ID OUTPUT_STRIP_TRAILING_WHITESPACE)
message(STATUS "Computed Git Hash: ${COMMIT_ID}")
add_definitions(-DGIT_HASH="${COMMIT_ID}") # Usable in source code.

include(${PICO_SDK_PATH}/pico_sdk_init.cmake)

project(schedule)

set(CMAKE_CXX_STANDARD 23)

## Configure the entire program to be copied from flash to RAM at the start.
## Since our binary size is small (<260KB), this is easier than marking every
## data structure and function definition called in core1 to run from RAM.
#set(PICO_COPY_TO_RAM 1)


# Enable try/catch exception interface.
#set(PICO_CXX_ENABLE_EXCEPTIONS 1)

# Compile for profiling/debugging/etc. Default: none enabled.
#add_definitions(-DDEBUG) # Warning! This severely slows down performance.

# initialize the Raspberry Pi Pico SDK
pico_sdk_init()

add_subdirectory(../../lib/etl build/etl)

include_directories(../../inc)

add_library(pwm_scheduler
    ../../src/pwm_scheduler.cpp
)

add_library(pwm_task
    ../../src/pwm_task.cpp
)

add_executable(${PROJECT_NAME}
    src/main.cpp
)

# Specify where to look for header files if they're not all in the same place.
#target_include_directories(${PROJECT_NAME} PUBLIC inc)
# Specify where to look for header files if they're all in one place.
include_directories(inc)

#set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -fverbose-asm")

# Link libraries to the targets that need them.
target_link_libraries(pwm_scheduler PUBLIC pico_stdlib etl::etl)
target_link_libraries(pwm_task PUBLIC hardware_gpio pico_stdlib)
target_link_libraries(${PROJECT_NAME} PUBLIC pico_stdlib
                      pwm_scheduler pwm_task)

# create map/bin/hex/uf2 file in addition to ELF.
pico_add_extra_outputs(${PROJECT_NAME})

pico_enable_stdio_usb(${PROJECT_NAME} 1)
