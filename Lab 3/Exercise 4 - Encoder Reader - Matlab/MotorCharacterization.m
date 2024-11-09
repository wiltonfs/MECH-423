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

%% Motor Positional Response to Positional Command
Kp = 200;
samplingTime_s = 1 / 1000;
sampleCount = 200;

positionTo500_counts = [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 12, 12, 13, 13, 14, 15, 15, 16, 16, 17, 18, 18, 19, 20, 20, 20, 21, 22, 22, 23, 24, 25, 25, 26, 27, 27, 28, 29, 30, 30, 31, 32, 33, 33, 34, 35, 36, 37, 37, 38, 39, 40, 41, 41, 42, 43, 44, 44, 44, 45, 46, 47, 48, 49, 49, 50, 51, 52, 53, 54, 54, 55, 56, 57, 58, 59, 59, 60, 61, 62, 63, 64, 65, 65, 66, 67, 68, 69, 70, 71, 71, 71, 72, 73, 74, 75, 76, 77, 78, 79, 79, 80, 81, 82, 83, 84, 85, 86, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 95, 96, 97, 98, 99, 100, 100, 101, 102, 103, 104, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 125, 126, 127, 128, 129, 130, 130, 131, 132, 133, 134, 135, 136, 137, 137, 138, 139, 140, 141];
positionTo1000_counts = [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 4, 4, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 12, 12, 13, 13, 14, 15, 15, 16, 16, 17, 18, 18, 19, 20, 20, 21, 22, 22, 23, 24, 24, 25, 25, 26, 27, 27, 28, 29, 30, 30, 31, 32, 33, 33, 33, 34, 35, 36, 36, 37, 38, 39, 40, 40, 41, 42, 43, 44, 44, 45, 46, 47, 48, 49, 49, 49, 50, 51, 52, 53, 54, 54, 55, 56, 57, 58, 59, 59, 60, 61, 62, 63, 64, 65, 66, 66, 67, 68, 69, 70, 71, 72, 72, 73, 74, 75, 76, 76, 77, 78, 79, 80, 80, 81, 82, 83, 84, 85, 86, 87, 88, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 106, 107, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 129, 130, 131, 132, 133, 134, 135, 135, 136, 137, 138, 139, 140];
time_ms = 0:1:(length(positionTo500_counts)-1);

Kp = 200;
samplingTime_s = 1 / 250;
sampleCount = 200;

positionTo500_counts = [ 0, 0, 0, 1, 3, 5, 7, 9, 12, 15, 18, 22, 25, 28, 32, 35, 39, 43, 47, 51, 55, 59, 63, 68, 72, 77, 80, 85, 89, 94, 98, 103, 107, 111, 115, 120, 125, 129, 134, 138, 142, 147, 151, 156, 161, 165, 169, 174, 179, 183, 188, 193, 197, 201, 205, 210, 215, 219, 224, 227, 231, 236, 240, 244, 248, 252, 256, 260, 264, 267, 271, 275, 279, 282, 285, 289, 292, 296, 299, 302, 305, 308, 311, 314, 317, 320, 323, 326, 328, 331, 334, 337, 339, 341, 344, 346, 349, 351, 353, 356, 358, 359, 361, 363, 365, 368, 370, 371, 373, 375, 377, 379, 381, 383, 384, 386, 388, 389, 391, 393, 394, 396, 397, 399, 401, 402, 404, 405, 406, 408, 409, 410, 412, 413, 415, 416, 417, 418, 419, 421, 422, 423, 424, 425, 426, 427, 428, 429, 430, 431, 432, 433, 434, 435, 436, 437, 438, 439, 440, 440, 441, 442, 443, 444, 444, 445, 446, 447, 447, 448, 449, 449, 450, 451, 452, 452, 453, 453, 454, 455, 455, 456, 456, 457, 458, 458, 459, 459, 460, 460, 461, 461, 462, 462, 463, 464, 464, 464, 465, 466];
positionTo1000_counts = [];
time_ms = 0:5:5*(length(positionTo500_counts)-1);

Kp = 500;
samplingTime_s = 1 / 250;
sampleCount = 200;

positionTo500_counts = [ 0, 0, 0, 0, 1, 2, 3, 4, 5, 7, 8, 10, 12, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31, 33, 36, 37, 39, 42, 44, 46, 48, 50, 52, 54, 57, 59, 61, 63, 66, 67, 70, 72, 74, 76, 79, 81, 83, 85, 87, 90, 92, 94, 96, 98, 100, 103, 105, 107, 110, 111, 114, 116, 118, 120, 123, 125, 127, 129, 131, 134, 136, 138, 141, 142, 145, 147, 149, 151, 154, 156, 158, 160, 162, 165, 167, 169, 171, 173, 176, 178, 180, 182, 185, 186, 188, 191, 193, 194, 196, 198, 200, 201, 203, 205, 206, 208, 209, 210, 211, 213, 214, 215, 216, 217, 218, 219, 220, 221, 221, 222, 223, 224, 224, 225, 226, 226, 227, 228, 228, 229, 229, 230, 230, 231, 231, 232, 233, 233, 234, 234, 235, 235, 236, 236, 236, 236, 237, 238, 238, 239, 241, 243, 245, 248, 251, 253, 256, 259, 263, 266, 270, 273, 276, 280, 283, 287, 290, 293, 296, 299, 302, 305, 308, 311, 313, 316, 319, 322, 324, 327, 330, 333, 335, 337, 340, 342, 344, 346, 347, 349, 350, 351, 353, 354, 355, 356, 357, 357, 358, 359];
time_ms = 0:5:5*(length(positionTo500_counts)-1);

%
% Fixed the weird non-saturation happening there
%
Kp = 500;
positionTo500_Kp500_counts = [ 0, 0, 1, 2, 4, 6, 8, 10, 12, 15, 18, 22, 25, 28, 32, 36, 40, 44, 48, 52, 56, 60, 64, 68, 73, 76, 80, 84, 89, 93, 98, 102, 107, 110, 115, 120, 124, 129, 133, 137, 142, 145, 150, 155, 159, 164, 168, 172, 177, 182, 186, 191, 195, 199, 204, 209, 213, 218, 223, 227, 231, 236, 241, 245, 250, 254, 258, 263, 268, 272, 277, 282, 286, 290, 295, 300, 304, 309, 314, 318, 322, 327, 332, 336, 341, 345, 350, 354, 359, 364, 368, 373, 377, 382, 385, 390, 394, 399, 401, 406, 410, 414, 418, 422, 426, 429, 432, 435, 439, 442, 445, 448, 450, 452, 455, 457, 459, 462, 463, 465, 467, 469, 470, 471, 473, 474, 475, 476, 478, 479, 480, 481, 482, 483, 483, 484, 485, 486, 487, 487, 488, 489, 489, 490, 490, 491, 492, 492, 493, 493, 494, 494, 495, 495, 496, 497, 497, 498, 498, 499, 499, 500, 500, 500, 501, 501, 501, 501, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502, 502];

Kp = 5000;
positionTo500_Kp5000_counts = [ 0, 0, 0, 0, 1, 3, 5, 6, 9, 12, 14, 18, 21, 24, 27, 31, 35, 39, 43, 47, 50, 54, 58, 63, 67, 71, 76, 79, 84, 88, 92, 97, 101, 105, 110, 114, 119, 123, 128, 133, 136, 141, 145, 150, 155, 159, 163, 168, 172, 177, 182, 186, 191, 195, 200, 204, 209, 214, 218, 223, 227, 231, 236, 241, 245, 250, 254, 259, 263, 268, 273, 276, 281, 285, 290, 294, 299, 304, 308, 312, 317, 322, 326, 331, 336, 340, 344, 349, 354, 358, 363, 368, 372, 376, 381, 386, 390, 395, 400, 404, 408, 413, 418, 423, 427, 431, 436, 440, 445, 450, 455, 459, 463, 468, 473, 477, 482, 487, 491, 495, 500, 504, 508, 510, 513, 514, 514, 513, 512, 510, 509, 507, 505, 503, 501, 499, 497, 496, 494, 494, 493, 493, 493, 494, 494, 495, 496, 498, 499, 500, 502, 503, 504, 505, 505, 506, 506, 506, 505, 504, 504, 503, 502, 501, 500, 499, 498, 498, 497, 496, 496, 496, 496, 496, 497, 497, 498, 498, 499, 499, 499, 500, 500, 500, 500, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501, 501];

Kp = 1000;
positionTo500_Kp1000_counts = [ 0, 0, 0, 0, 1, 3, 5, 6, 9, 12, 15, 18, 21, 25, 28, 31, 35, 39, 43, 47, 50, 54, 58, 63, 67, 71, 76, 79, 84, 88, 93, 98, 102, 106, 110, 115, 120, 124, 129, 133, 137, 142, 146, 151, 156, 160, 164, 169, 174, 178, 183, 188, 192, 196, 201, 205, 209, 214, 219, 223, 227, 232, 237, 241, 246, 251, 254, 259, 264, 269, 273, 278, 283, 287, 291, 296, 301, 305, 310, 314, 319, 323, 328, 333, 338, 342, 346, 351, 356, 360, 365, 370, 374, 378, 383, 388, 392, 397, 402, 406, 410, 415, 420, 425, 428, 432, 437, 442, 446, 451, 456, 460, 464, 468, 472, 476, 479, 483, 486, 488, 491, 493, 495, 497, 499, 500, 502, 503, 505, 506, 507, 507, 508, 508, 508, 508, 508, 508, 508, 508, 507, 507, 507, 507, 507, 506, 506, 506, 505, 505, 504, 504, 504, 503, 503, 502, 502, 501, 501, 501, 500, 500, 500, 500, 500, 500, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499, 499];


time_ms = 0:5:5*(length(positionTo500_counts)-1);
figure; hold on;
plot(time_ms, positionTo500_Kp500_counts);
plot(time_ms, positionTo500_Kp5000_counts);
plot(time_ms, positionTo500_Kp1000_counts);
title("Motor Response to Positional Command (1ms Sample Time)")
xlabel("Time [ms]");
ylabel("Motor Position [counts]");
legend('Kp = 500', 'Kp = 5000', 'Kp = 1000', 'location', 'northwest');
saveas(gcf, 'ControlCharacterization.png');

positionTo250_Kp1000_counts = [ 0, 0, 0, 0, 2, 3, 5, 7, 10, 13, 16, 19, 23, 25, 29, 33, 37, 41, 45, 49, 52, 56, 61, 65, 69, 74, 77, 82, 85, 90, 94, 99, 103, 107, 112, 116, 121, 126, 130, 134, 139, 143, 148, 153, 157, 162, 166, 170, 175, 180, 185, 189, 193, 198, 202, 207, 211, 215, 219, 222, 226, 229, 233, 236, 239, 242, 244, 247, 249, 251, 253, 254, 256, 258, 259, 260, 260, 260, 261, 261, 261, 261, 261, 261, 261, 260, 260, 260, 259, 259, 258, 258, 257, 257, 257, 256, 256, 254, 254, 254, 254, 253, 253, 252, 252, 251, 251, 251, 250, 250, 250, 250, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249, 249];

time_ms = 0:5:5*(length(positionTo500_counts)-1);
figure; hold on;
plot(time_ms, positionTo250_Kp500_counts);
plot(time_ms, positionTo250_Kp5000_counts);
plot(time_ms, positionTo250_Kp1000_counts);
title("Motor Response to Positional Command (1ms Sample Time)")
xlabel("Time [ms]");
ylabel("Motor Position [counts]");
legend('Kp = 500', 'Kp = 5000', 'Kp = 1000', 'location', 'northwest');
saveas(gcf, 'ControlCharacterization_2.png');



