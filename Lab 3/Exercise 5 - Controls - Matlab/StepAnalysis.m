% MECH 423
% Exercise 4
% Step Analysis: Characterizing the DC Motor
% Felix Wilton      Lazar Stanojevic
% 2024-10-31

% ChatGPT and Matlab forums were referenced to produce parts of this code
clear;close all;clc;

path = "../Exercise 5 - Controls - C#/Data/PWM sweep.csv";

data = readtable(path, 'NumHeaderLines', 1);
time_ms = data{:, 1};
pwm = data{:, 2};
position_counts = data{:, 3};
velocity_Hz = data{:, 4};

figure; hold on;
yyaxis left;
plot(time_ms, pwm / 65000);
ylabel('PWM [%]');
ylim([0, 1.25]);
yyaxis right;
plot(time_ms, position_counts);
ylabel('Position [counts]');
xlabel('Time [ms]');
title("Positional Response");



figure; hold on;
yyaxis left;
plot(time_ms, pwm / 65000);
ylabel('PWM [%]');
ylim([0, 1.25]);
yyaxis right;
plot(time_ms, velocity_Hz);
ylabel('Velocity [Hz]');
xlabel('Time [ms]');
title("Velocity Response");