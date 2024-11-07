% MECH 423
% Exercise 4
% Motor Speed vs Duty Cycle
% Felix Wilton      Lazar Stanojevic
% 2024-11-06

% ChatGPT and Matlab forums were referenced to produce parts of this code
clear;close all;clc;

%%
PWMs = [0, 13000, 19500, 26000, 32500, 39000, 45500, 58500, 65000];
%speed_Hz = [0, 0.7742205, 1.129943, 1.506591, 1.883239, 2.301737, 2.720234, 3.578155, 3.954802];
speed_Hz = [0. 0.7458, 1.0884, 1.4512, 1.8141, 2.2172, 2.6203, 3.4467, 3.8095];
%old_counts_per_cycle = 236; % Estimate used to generate data
%new_counts_per_cycle = 245; % From datasheet
PWM_max = 65535;
%speed_Hz = speed_Hz * old_counts_per_cycle / new_counts_per_cycle;
PWM_percent = PWMs/PWM_max;

p = polyfit(PWM_percent, speed_Hz, 1);
fprintf("Line of Best Fit: w [Hz] = %.4f*PWM [%%] + %.4f\n", p(1), p(2));

figure;
scatter(PWM_percent, speed_Hz, 100, 'r', 'x');
hold on;
plot(PWM_percent, polyval(p, PWM_percent), 'k--');
legend("Data", "Fit", 'location', 'northwest');
ylabel('Motor Speed [Hz]');
ylim([0 4]);
xlabel('PWM Duty Cycle [%]');
title("DC Motor Speed vs PWM at 5V");
saveas(gcf, 'SpeedPWMRelationship.png');