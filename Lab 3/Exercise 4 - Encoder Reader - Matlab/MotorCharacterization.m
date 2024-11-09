%% Motor Positional Response to Step Input
close all; clear; clc;
DutyCycles = [25, 50, 75, 100];

data = readtable("StepInput_1ms.csv", 'NumHeaderLines', 1);
time_ms = 0:1:(size(data(:,1))-1);
figure;
hold on;
for i = 1:length(DutyCycles)
    plot(time_ms, data{:, i});
end
title("Motor Response to Step Inputs (1ms Sample Time)")
legend('25% Duty Cycle', '50% Duty Cycle', '75% Duty Cycle', '100% Duty Cycle', 'location', 'northwest');
xlabel("Time [ms]");
ylabel("Motor Position [counts]");
saveas(gcf, 'StepInput_1ms.png');

data = readtable("StepInput_5ms.csv", 'NumHeaderLines', 1);
time_ms = 0:5:5*(size(data(:,1))-1);
figure;
hold on;
for i = 1:length(DutyCycles)
    plot(time_ms, data{:, i});
end
title("Motor Response to Step Inputs (5ms Sample Time)")
legend('25% Duty Cycle', '50% Duty Cycle', '75% Duty Cycle', '100% Duty Cycle', 'location', 'northwest');
xlabel("Time [ms]");
ylabel("Motor Position [counts]");
saveas(gcf, 'StepInput_5ms.png');


%% Motor Velocity Response to Step Input